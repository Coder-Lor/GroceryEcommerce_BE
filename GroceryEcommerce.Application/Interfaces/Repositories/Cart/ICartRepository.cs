using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Cart;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Cart;

public interface ICartRepository
{
    // Shopping Cart operations
    Task<Result<ShoppingCart?>> GetShoppingCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCart>> CreateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteShoppingCartAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default);

    // Shopping Cart Item operations
    Task<Result<List<ShoppingCartItem>>> GetShoppingCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItem?>> GetShoppingCartItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItem?>> GetShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItem>> AddShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShoppingCartItemQuantityAsync(Guid itemId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveShoppingCartItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);

    // Wishlist operations
    Task<Result<Wishlist?>> GetWishlistByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<Wishlist>> CreateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default);

    // Wishlist Item operations
    Task<Result<List<WishlistItem>>> GetWishlistItemsAsync(Guid wishlistId, CancellationToken cancellationToken = default);
    Task<Result<WishlistItem?>> GetWishlistItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<WishlistItem?>> GetWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<WishlistItem>> AddWishlistItemAsync(WishlistItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveWishlistItemAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsProductInWishlistAsync(Guid userId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);

    // Abandoned Cart operations
    Task<Result<List<AbandonedCart>>> GetAbandonedCartsAsync(CancellationToken cancellationToken = default);
    Task<Result<AbandonedCart?>> GetAbandonedCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<AbandonedCart>> CreateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAbandonedCartAsync(Guid abandonedCartId, CancellationToken cancellationToken = default);
    Task<Result<List<AbandonedCart>>> GetAbandonedCartsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    // Cart calculations
    Task<Result<decimal>> CalculateCartTotalAsync(Guid cartId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCartItemCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default);
}
