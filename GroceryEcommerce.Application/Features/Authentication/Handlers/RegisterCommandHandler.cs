using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Handlers;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IAuthenticationRepository authenticationRepository,
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RegisterCommandHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHashService _passwordHashService = passwordHashService;
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting user registration for email: {Email}", request.Email);
        var emailExitsResult = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExitsResult is { IsSuccess: true, Data: true })
        {
            _logger.LogWarning("Registration failed: Email {Email} already exists}", request.Email);
            return Result<RegisterResponse>.Failure("Email already exists");
        }

        var hasedPassword = _passwordHashService.HashPassword(request.Password);
        var user = new User
        {
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
            CreatedAt = default,
            UpdatedAt = null,
            LastLoginAt = null
        };

        var addUserResult = await _userRepository.AddAsync(user, cancellationToken);
        if (!addUserResult.IsSuccess)
        {
            _logger.LogError("Failed to add user to database: {Error}", addUserResult.ErrorCode);
            return Result<RegisterResponse>.Failure("Failed to create user account");
        }
        
        // gen tokens 
        var accessToken = await _tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, new List<string> {"User"});
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId);

        var refreshTokenEntity = new RefreshToken
        {
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
            return Result<RegisterResponse>.Failure("Failed to create user session");
        }

        await _unitOfWork.CommitAsync(cancellationToken);
        _logger.LogInformation("User registered successfully: {UserId}", user.UserId);
        

        var response = new RegisterResponse
        {
            UserId = user.UserId.ToString(),
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return Result<RegisterResponse>.Success(response);
    }
}