using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IUserRoleRepository
{
    // Basic CRUD operations
    Task<Result<UserRole?>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<UserRole?>> GetByNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Result<List<UserRole>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<UserRole>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserRole>> CreateAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default);
    
    // Role management operations
    Task<Result<bool>> ExistsAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<List<UserRole>>> GetActiveRolesAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> IsRoleInUseAsync(Guid roleId, CancellationToken cancellationToken = default);
}
