using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Cart;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IWishlistItemRepository
{
    // Basic CRUD operations
    Task<Result<WishlistItem?>> GetByIdAsync(Guid wishlistItemId, CancellationToken cancellationToken = default);
    Task<Result<List<WishlistItem>>> GetByWishlistIdAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<WishlistItem?>> GetByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<WishlistItem>> CreateAsync(WishlistItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(WishlistItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid wishlistItemId, CancellationToken cancellationToken = default);
    
    // Wishlist item management operations
    Task<Result<bool>> ExistsAsync(Guid wishlistItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<List<WishlistItem>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
