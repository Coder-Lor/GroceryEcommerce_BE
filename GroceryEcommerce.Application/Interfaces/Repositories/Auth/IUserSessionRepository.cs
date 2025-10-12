using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IUserSessionRepository
{
    // Basic CRUD operations
    Task<Result<UserSession?>> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Result<UserSession?>> GetByTokenAsync(string sessionToken, CancellationToken cancellationToken = default);
    Task<Result<List<UserSession>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserSession>> CreateAsync(UserSession session, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(UserSession session, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default);
    
    // Session management operations
    Task<Result<bool>> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RevokeExpiredSessionsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<UserSession>>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsSessionValidAsync(string sessionToken, CancellationToken cancellationToken = default);
    Task<Result<int>> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
