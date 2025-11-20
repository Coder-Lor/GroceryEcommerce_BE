using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Auth;

public interface IUserAddressRepository
{
    // Basic CRUD operations
    Task<Result<UserAddress?>> GetByIdAsync(Guid addressId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<UserAddress>>> GetByUserIdAsync( PagedRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<UserAddress?>> GetDefaultAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(UserAddress address, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(UserAddress address, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid addressId, CancellationToken cancellationToken = default);
    
    // Address management operations
    Task<Result<bool>> SetDefaultAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<UserAddress>>> GetAddressesByTypeAsync(PagedRequest request, Guid userId, short addressType, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid addressId, CancellationToken cancellationToken = default);
}
