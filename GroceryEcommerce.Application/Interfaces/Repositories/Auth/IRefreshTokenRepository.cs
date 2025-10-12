using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IRefreshTokenRepository
{
    // Basic CRUD operations
    Task<Result<RefreshToken?>> GetByIdAsync(Guid tokenId, CancellationToken cancellationToken = default);
    Task<Result<RefreshToken?>> GetByTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default);
    Task<Result<List<RefreshToken>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<RefreshToken>>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<RefreshToken>> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid tokenId, CancellationToken cancellationToken = default);
    
    // Token management operations
    Task<Result<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> IsTokenValidAsync(string refreshTokenValue, CancellationToken cancellationToken = default);
}
