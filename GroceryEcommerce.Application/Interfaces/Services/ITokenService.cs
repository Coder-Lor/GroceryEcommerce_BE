using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(Guid userId, string email, IList<string> roles);
    Task<string> GenerateRefreshTokenAsync(Guid userId, string ipAddress = null);
    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string refreshToken, string? ipAddress = null);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId);
    Task<bool> ValidateRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null);
    Task RevokeAllUserRefreshTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
    Task<bool> IsTokenRevokedAsync(string token);
    Task<string> GetUserIdFromExpiredToken(string token);
}