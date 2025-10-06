using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IUserRoleAssignmentRepository
{
    // Basic CRUD operations
    Task<Result<UserRoleAssignment?>> GetByIdAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<List<UserRoleAssignment>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<UserRoleAssignment>>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<UserRoleAssignment>> CreateAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    
    // Role assignment management operations
    Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);
    Task<Result<List<string>>> GetUserRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<Guid>>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<Guid>>> GetUsersByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<Result<List<Guid>>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveAllRolesFromUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
