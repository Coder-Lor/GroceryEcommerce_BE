using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

public sealed class 
    LoginCommandHandler(
    IAuthenticationRepository authenticationRepository,
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    ITokenService tokenService,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private const int MaxFailedAttempts = 5;
    
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for: {EmailOrUsername}", request.EmailOrUsername);
        
        var failedAttempts = await authenticationRepository.GetFailedLoginAttemptsAsync(request.EmailOrUsername, cancellationToken);
        if (failedAttempts is { IsSuccess: true, Data: > MaxFailedAttempts })
        {
            logger.LogWarning("Login failed: Too many failed login attempts for {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Too many failed login attempts");
        }
        
        var userResult = await userRepository.GetUserByEmailOrUsernameAsync(request.EmailOrUsername, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data is null)
        {
            await authenticationRepository.RecordFailedLoginAttemptAsync(request.EmailOrUsername, cancellationToken);
            logger.LogWarning("Login failed: User not found - {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Invalid credentials");
        }

        var user = userResult.Data;
        if (!passwordHashService.VerifyPassword(request.Password, user.PasswordHash)) 
        {
            await authenticationRepository.RecordFailedLoginAttemptAsync(request.EmailOrUsername, cancellationToken);
            logger.LogWarning("Login failed: Invalid credentials - {EmailOrUsername}", request.EmailOrUsername);
            return Result<LoginResponse>.Failure("Invalid credentials");
        }
        
        await authenticationRepository.ResetFailedLoginAttemptsAsync(request.EmailOrUsername, cancellationToken);
        
        var userRoles = await authenticationRepository.GetUserRolesAsync(user.UserId, cancellationToken);
        var accessToken = await tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, new List<string> {"User"});
        var refreshToken = await tokenService.GenerateRefreshTokenAsync(user.UserId);
        await authenticationRepository.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow, cancellationToken);

        
        // store refesh token in cache
        var tokenExpity = DateTime.UtcNow.AddDays(7);
        await authenticationRepository.CreateRefreshTokenAsync(user.UserId, refreshToken, tokenExpity, cancellationToken);     

        logger.LogInformation("User logged in successfully: {UserId}", user.UserId);

        var response = new LoginResponse
        {
            UserId = user.UserId.ToString(),
            Username = user.Username,
            Role = userRoles.Data.FirstOrDefault() ?? "Admin",
            Email = user.Email,
            Token = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return Result<LoginResponse>.Success(response);

    }
}