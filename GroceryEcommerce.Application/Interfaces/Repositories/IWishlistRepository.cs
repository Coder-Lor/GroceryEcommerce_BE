using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Cart;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IWishlistRepository
{
    // Basic CRUD operations
    Task<Result<Wishlist?>> GetByIdAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<List<Wishlist>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Wishlist?>> GetDefaultWishlistByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Wishlist>> CreateAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    
    // Wishlist management operations
    Task<Result<bool>> SetDefaultWishlistAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveDefaultWishlistAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<Wishlist>>> GetPublicWishlistsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Wishlist>>> GetPublicWishlistsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetWishlistCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
