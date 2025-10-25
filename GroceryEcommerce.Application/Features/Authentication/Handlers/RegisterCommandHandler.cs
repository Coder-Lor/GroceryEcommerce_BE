using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GroceryEcommerce.Application.Features.Authentication.Handlers;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IAuthenticationRepository authenticationRepository,
    IPasswordHashService passwordHashService,
    IEmailService emailService,
    ITokenService tokenService,
    IUnitOfWorkService unitOfWorkService,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUnitOfWorkService _unitOfWorkService = unitOfWorkService;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RegisterCommandHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;   
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var hashEmail = _passwordHashService.HashPassword(request.Email);
        _logger.LogInformation("Starting user registration for email: {Email}", hashEmail);
        try
        {
            var validationStopwatch = Stopwatch.StartNew();
            var existenceResult = await _userRepository.CheckUserExistenceAsync(request.Email, request.Username, cancellationToken);
            if (!existenceResult.IsSuccess)
            {
                _logger.LogWarning("Registration failed: Could not validate user existence for Email {Email} and Username {Username}", hashEmail, request.Username);
                return Result<RegisterResponse>.Failure("Failed to validate user existence");
            }

            var (emailExists, usernameExists) = existenceResult.Data;
            if (emailExists)
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists", hashEmail);
                return Result<RegisterResponse>.Failure("Email already exists");
            }
            if (usernameExists)
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", request.Username);
                return Result<RegisterResponse>.Failure("Username already exists");           
            }
            validationStopwatch.Stop();
            _logger.LogInformation("Validation completed in {ValidationTime}ms", validationStopwatch.ElapsedMilliseconds);

            var userCreationStopwatch = Stopwatch.StartNew();
            var hasedPassword = _passwordHashService.HashPassword(request.Password);
            userCreationStopwatch.Stop();
            _logger.LogInformation("User creation completed in {UserCreationTime}ms", userCreationStopwatch.ElapsedMilliseconds);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hasedPassword,
                FirstName = null,
                LastName = null,
                PhoneNumber = null,
                DateOfBirth = null,
                Status = 1, // active
                EmailVerified = false,
                PhoneVerified = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                LastLoginAt = null
            };
            
            string accessToken = "";
            string refreshToken = "";

            var dbTransactionStopwatch = Stopwatch.StartNew();
            await _unitOfWorkService.ExecuteInTransactionAsync(async _ =>
            {
                var addUserResult = await _userRepository.AddAsync(user, cancellationToken);
                if (!addUserResult.IsSuccess)
                {
                    throw new Exception($"Failed to add user: {addUserResult.ErrorCode}");
                }
                
                accessToken = await _tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, new List<string> {"User"});
                refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId);
                
                var refreshTokenEntity = new RefreshToken
                {
                    TokenId = Guid.NewGuid(),
                    UserId = user.UserId,
                    RefreshTokenValue = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    Revoked = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByIp = null
                };

                var saveTokenResult = await _authenticationRepository.SaveRefreshTokenAsync(refreshTokenEntity, cancellationToken);
                if (!saveTokenResult.IsSuccess)
                {
                    _logger.LogError("Failed to save refresh token: {Error}", saveTokenResult.ErrorCode);
                    throw new InvalidOperationException($"Failed to save refresh token: {saveTokenResult.ErrorCode}");
                }
                
            }, cancellationToken);
            dbTransactionStopwatch.Stop();
            _logger.LogInformation("Database transaction completed in {DbTransactionTime}ms", dbTransactionStopwatch.ElapsedMilliseconds);
            
            var verifyToken = Guid.NewGuid().ToString();
            var emailVerificationStopwatch = Stopwatch.StartNew();
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendEmailVerificationAsync(request.Email, verifyToken);
                    _logger.LogInformation("Verification email sent successfully to {Email}", hashEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send verification email to {Email}", hashEmail);
                }
            }, CancellationToken.None); 
            emailVerificationStopwatch.Stop();
            _logger.LogInformation("Email verification completed in {EmailVerificationTime}ms", emailVerificationStopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();
            _logger.LogInformation("User registration completed in {TotalElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            var response = new RegisterResponse
            {
                UserId = user.UserId.ToString(),
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            

            return Result<RegisterResponse>.Success(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogInformation("Total time taken: {TotalTime}ms", stopwatch.ElapsedMilliseconds);
            _logger.LogError(ex, "Error registering user: {Email}", request.Email);
            return Result<RegisterResponse>.Failure("An error occurred while registering user");
        }
    }
}