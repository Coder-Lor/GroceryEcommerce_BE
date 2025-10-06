using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Infrastructure.Services;

public class TokenService : ITokenService
{
    public Task<string> GenerateAccessTokenAsync(Guid userId, string email, IList<string> roles)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateRefreshTokenAsync(Guid userId, string ipAddress = null)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, string refreshToken, string? ipAddress = null)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string token, string? replacedByToken = null)
    {
        throw new NotImplementedException();
    }

    public Task RevokeAllUserRefreshTokensAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task CleanupExpiredTokensAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenRevokedAsync(string token)
    {
        throw new NotImplementedException();
    }

    public string GetUserIdFromExpiredToken(string token)
    {
        throw new NotImplementedException();
    }
}