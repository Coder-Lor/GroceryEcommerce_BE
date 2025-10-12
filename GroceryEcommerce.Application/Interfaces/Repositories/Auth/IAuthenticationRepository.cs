using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IAuthenticationRepository
{
    Task<Result<User?>> ValidateUserCredentialsAsync(string emailOrUsername, string password, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken = default);
    Task<Result<bool>> SaveRefreshTokenAsync(RefreshToken refreshTokens, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken = default);
    Task<Result<string?>> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateLastLoginAsync(Guid userId, DateTime lastLogin, CancellationToken cancellationToken = default);
    Task<Result<bool>> RecordFailedLoginAttemptAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<Result<int>> GetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<Result<bool>> ResetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
