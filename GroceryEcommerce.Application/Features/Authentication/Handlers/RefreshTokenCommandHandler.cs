using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Authentication.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Authentication.Handlers;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    ITokenService tokenService,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly ITokenService _tokenService = tokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refresh token request received");
        
        var isValid = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (!isValid)
        {
            _logger.LogWarning("Invalid refresh token provided");
            return Result<RefreshTokenResponse>.Failure("Invalid refresh token");
        }
        
        var refreshTokenResult = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (!refreshTokenResult.IsSuccess || refreshTokenResult.Data is null)
        {
            _logger.LogWarning("Refresh token not found in database");
            return Result<RefreshTokenResponse>.Failure("Refresh token not found");
        }

        var refreshToken = refreshTokenResult.Data;
        
        if (refreshToken.Revoked)
        {
            _logger.LogWarning("Refresh token has been revoked");
            return Result<RefreshTokenResponse>.Failure("Refresh token has been revoked");
        }

        // Check if token is expired
        if (refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Refresh token has expired");
            return Result<RefreshTokenResponse>.Failure("Refresh token has expired");
        }

        // Get user information
        var userResult = await _userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
        if (!userResult.IsSuccess || userResult.Data is null)
        {
            _logger.LogWarning("User not found for refresh token: {UserId}", refreshToken.UserId);
            return Result<RefreshTokenResponse>.Failure("User not found");
        }

        var user = userResult.Data;

        var userRoles = new List<string> { "User" };
        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(user.UserId, user.Email, userRoles);
        
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user.UserId);
        
        await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken, newRefreshToken);
        
        var tokenValidated = await _tokenService.CreateRefreshTokenInDatabaseAsync(user.UserId, newRefreshToken);
        
        if (!tokenValidated)
        {
            _logger.LogError("Failed to validate and create new refresh token for user: {UserId}", user.UserId);
            return Result<RefreshTokenResponse>.Failure("Failed to create new refresh token");
        }

        _logger.LogInformation("Tokens refreshed successfully for user: {UserId}", user.UserId);

        var response = new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return Result<RefreshTokenResponse>.Success(response);
    }
}