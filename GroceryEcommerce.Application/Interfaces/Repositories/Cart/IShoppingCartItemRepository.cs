using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Cart;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Cart;

public interface IShoppingCartItemRepository
{
    // Basic CRUD operations
    Task<Result<ShoppingCartItem?>> GetByIdAsync(Guid cartItemId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ShoppingCartItem>>> GetByCartIdAsync(PagedRequest request, Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItem?>> GetByProductAsync(Guid cartId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItem>> CreateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid cartItemId, CancellationToken cancellationToken = default);
    
    // Cart item management operations
    Task<Result<bool>> ExistsAsync(Guid cartItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateQuantityAsync(Guid cartItemId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveByProductAsync(Guid cartId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByCartAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetCartTotalAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearCartAsync(Guid cartId, CancellationToken cancellationToken = default);
}
