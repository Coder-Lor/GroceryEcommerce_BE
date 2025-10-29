using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

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
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var hashEmail = passwordHashService.HashPassword(request.Email);
        logger.LogInformation("Starting user registration for email: {Email}", hashEmail);
        try
        {
            var validationStopwatch = Stopwatch.StartNew();
            var existenceResult = await userRepository.CheckUserExistenceAsync(request.Email, request.Username, cancellationToken);
            if (!existenceResult.IsSuccess)
            {
                logger.LogWarning("Registration failed: Could not validate user existence for Email {Email} and Username {Username}", hashEmail, request.Username);
                return Result<RegisterResponse>.Failure("Failed to validate user existence");
            }

            var (emailExists, usernameExists) = existenceResult.Data;
            if (emailExists)
            {
                logger.LogWarning("Registration failed: Email {Email} already exists", hashEmail);
                return Result<RegisterResponse>.Failure("Email already exists");
            }
            if (usernameExists)
            {
                logger.LogWarning("Registration failed: Username {Username} already exists", request.Username);
                return Result<RegisterResponse>.Failure("Username already exists");           
            }
            validationStopwatch.Stop();
            logger.LogInformation("Validation completed in {ValidationTime}ms", validationStopwatch.ElapsedMilliseconds);

            var userCreationStopwatch = Stopwatch.StartNew();
            var hasedPassword = passwordHashService.HashPassword(request.Password);
            userCreationStopwatch.Stop();
            logger.LogInformation("User creation completed in {UserCreationTime}ms", userCreationStopwatch.ElapsedMilliseconds);

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
            await unitOfWorkService.ExecuteInTransactionAsync(async _ =>
            {
                var addUserResult = await userRepository.AddAsync(user, cancellationToken);
                if (!addUserResult.IsSuccess)
                {
                    throw new Exception($"Failed to add user: {addUserResult.ErrorCode}");
                }
                
                accessToken = await tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, new List<string> {"User"});
                refreshToken = await tokenService.GenerateRefreshTokenAsync(user.UserId);
                
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

                var saveTokenResult = await authenticationRepository.SaveRefreshTokenAsync(refreshTokenEntity, cancellationToken);
                if (!saveTokenResult.IsSuccess)
                {
                    logger.LogError("Failed to save refresh token: {Error}", saveTokenResult.ErrorCode);
                    throw new InvalidOperationException($"Failed to save refresh token: {saveTokenResult.ErrorCode}");
                }
                
            }, cancellationToken);
            dbTransactionStopwatch.Stop();
            logger.LogInformation("Database transaction completed in {DbTransactionTime}ms", dbTransactionStopwatch.ElapsedMilliseconds);
            
            var verifyToken = Guid.NewGuid().ToString();
            var emailVerificationStopwatch = Stopwatch.StartNew();
            _ = Task.Run(async () =>
            {
                try
                {
                    await emailService.SendEmailVerificationAsync(request.Email, verifyToken);
                    logger.LogInformation("Verification email sent successfully to {Email}", hashEmail);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send verification email to {Email}", hashEmail);
                }
            }, CancellationToken.None); 
            emailVerificationStopwatch.Stop();
            logger.LogInformation("Email verification completed in {EmailVerificationTime}ms", emailVerificationStopwatch.ElapsedMilliseconds);
            
            stopwatch.Stop();
            logger.LogInformation("User registration completed in {TotalElapsedMs}ms", stopwatch.ElapsedMilliseconds);

            var response = new RegisterResponse
            {
                UserId = user.UserId.ToString(),
                Username = user.Username,
                Role = "User",
                Email = user.Email,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };

            

            return Result<RegisterResponse>.Success(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogInformation("Total time taken: {TotalTime}ms", stopwatch.ElapsedMilliseconds);
            logger.LogError(ex, "Error registering user: {Email}", request.Email);
            return Result<RegisterResponse>.Failure("An error occurred while registering user");
        }
    }
}