using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Cart;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ICartService
{
    // Shopping Cart services
    Task<Result<ShoppingCartDto?>> GetShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartDto>> CreateShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> CalculateCartTotalAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCartItemCountAsync(Guid userId, CancellationToken cancellationToken = default);

    // Shopping Cart Item services
    Task<Result<List<ShoppingCartItemDto>>> GetShoppingCartItemsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ShoppingCartItemDto>> AddItemToCartAsync(AddToCartRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCartItemQuantityAsync(Guid userId, Guid itemId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveItemFromCartAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveItemByProductAsync(Guid userId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCartItemsAsync(Guid userId, CancellationToken cancellationToken = default);

    // Wishlist services
    Task<Result<WishlistDto?>> GetWishlistAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<WishlistDto>> CreateWishlistAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<WishlistItemDto>>> GetWishlistItemsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<WishlistItemDto>> AddItemToWishlistAsync(Guid userId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveItemFromWishlistAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveItemByProductFromWishlistAsync(Guid userId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsProductInWishlistAsync(Guid userId, Guid productId, Guid? variantId = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> MoveItemToCartAsync(Guid userId, Guid wishlistItemId, int quantity = 1, CancellationToken cancellationToken = default);

    // Abandoned Cart services
    Task<Result<List<AbandonedCartDto>>> GetAbandonedCartsAsync(CancellationToken cancellationToken = default);
    Task<Result<AbandonedCartDto?>> GetAbandonedCartByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAbandonedCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAbandonedCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAbandonedCartAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<AbandonedCartDto>>> GetAbandonedCartsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

    // Cart utilities
    Task<Result<bool>> MergeCartsAsync(Guid userId, Guid guestCartId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ApplyCouponAsync(Guid userId, string couponCode, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveCouponAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CartSummaryDto>> GetCartSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SaveCartForLaterAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RestoreSavedCartAsync(Guid userId, CancellationToken cancellationToken = default);
}
