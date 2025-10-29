using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Auth.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Auth.Authentication.Handlers;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Refresh token request received");
        
        var isValid = await tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (!isValid)
        {
            logger.LogWarning("Invalid refresh token provided");
            return Result<RefreshTokenResponse>.Failure("Invalid refresh token");
        }
        
        var refreshTokenResult = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (!refreshTokenResult.IsSuccess || refreshTokenResult.Data is null)
        {
            logger.LogWarning("Refresh token not found in database");
            return Result<RefreshTokenResponse>.Failure("Refresh token not found");
        }

        var refreshToken = refreshTokenResult.Data;
        
        if (refreshToken.Revoked)
        {
            logger.LogWarning("Refresh token has been revoked");
            return Result<RefreshTokenResponse>.Failure("Refresh token has been revoked");
        }
        
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Refresh token has expired");
            return Result<RefreshTokenResponse>.Failure("Refresh token has expired");
        }
        
        var userResult = await userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data is null)
        {
            logger.LogWarning("User not found for refresh token: {UserId}", refreshToken.UserId);
            return Result<RefreshTokenResponse>.Failure("User not found");
        }

        var user = userResult.Data;

        var userRoles = new List<string> { "User" };
        var newAccessToken = await tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, userRoles);
        
        var newRefreshToken = await tokenService.GenerateRefreshTokenAsync(user.UserId);
        
        await tokenService.RevokeRefreshTokenAsync(request.RefreshToken, newRefreshToken);
        
        var tokenValidated = await tokenService.CreateRefreshTokenInDatabaseAsync(user.UserId, newRefreshToken);
        
        if (!tokenValidated)
        {
            logger.LogError("Failed to validate and create new refresh token for user: {UserId}", user.UserId);
            return Result<RefreshTokenResponse>.Failure("Failed to create new refresh token");
        }

        logger.LogInformation("Tokens refreshed successfully for user: {UserId}", user.UserId);

        var response = new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return Result<RefreshTokenResponse>.Success(response);
    }
}