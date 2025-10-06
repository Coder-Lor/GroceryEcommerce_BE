using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Cart;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    #region Shopping Cart

    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<Result<ShoppingCartDto>>> GetShoppingCart(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetShoppingCartAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shopping cart for user: {UserId}", userId);
            return StatusCode(500, Result<ShoppingCartDto>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}")]
    public async Task<ActionResult<Result<ShoppingCartDto>>> CreateShoppingCart(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.CreateShoppingCartAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetShoppingCart), new { userId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating shopping cart for user: {UserId}", userId);
            return StatusCode(500, Result<ShoppingCartDto>.Failure("Internal server error"));
        }
    }

    [HttpDelete("users/{userId:guid}")]
    public async Task<ActionResult<Result<bool>>> ClearShoppingCart(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.ClearShoppingCartAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing shopping cart for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpGet("users/{userId:guid}/total")]
    public async Task<ActionResult<Result<decimal>>> GetCartTotal(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.CalculateCartTotalAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating cart total for user: {UserId}", userId);
            return StatusCode(500, Result<decimal>.Failure("Internal server error"));
        }
    }

    [HttpGet("users/{userId:guid}/count")]
    public async Task<ActionResult<Result<int>>> GetCartItemCount(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetCartItemCountAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart item count for user: {UserId}", userId);
            return StatusCode(500, Result<int>.Failure("Internal server error"));
        }
    }

    [HttpGet("users/{userId:guid}/summary")]
    public async Task<ActionResult<Result<CartSummaryDto>>> GetCartSummary(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetCartSummaryAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart summary for user: {UserId}", userId);
            return StatusCode(500, Result<CartSummaryDto>.Failure("Internal server error"));
        }
    }

    #endregion

    #region Shopping Cart Items

    [HttpGet("users/{userId:guid}/items")]
    public async Task<ActionResult<Result<List<ShoppingCartItemDto>>>> GetShoppingCartItems(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetShoppingCartItemsAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shopping cart items for user: {UserId}", userId);
            return StatusCode(500, Result<List<ShoppingCartItemDto>>.Failure("Internal server error"));
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<Result<ShoppingCartItemDto>>> AddItemToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.AddItemToCartAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetShoppingCartItems), new { userId = request.UserId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart for user: {UserId}", request.UserId);
            return StatusCode(500, Result<ShoppingCartItemDto>.Failure("Internal server error"));
        }
    }

    [HttpPut("items/{itemId:guid}/quantity")]
    public async Task<ActionResult<Result<bool>>> UpdateCartItemQuantity(
        Guid itemId, 
        [FromBody] UpdateQuantityRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.UpdateCartItemQuantityAsync(request.UserId, itemId, request.Quantity, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item quantity: {ItemId}", itemId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemFromCart(
        Guid itemId, 
        [FromBody] RemoveItemRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RemoveItemFromCartAsync(request.UserId, itemId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from cart: {ItemId}", itemId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("users/{userId:guid}/products/{productId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemByProduct(
        Guid userId, 
        Guid productId, 
        [FromQuery] Guid? variantId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RemoveItemByProductAsync(userId, productId, variantId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from cart: {ProductId} for user: {UserId}", productId, userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/validate")]
    public async Task<ActionResult<Result<bool>>> ValidateCartItems(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.ValidateCartItemsAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating cart items for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion

    #region Wishlist

    [HttpGet("users/{userId:guid}/wishlist")]
    public async Task<ActionResult<Result<WishlistDto>>> GetWishlist(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetWishlistAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist for user: {UserId}", userId);
            return StatusCode(500, Result<WishlistDto>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/wishlist")]
    public async Task<ActionResult<Result<WishlistDto>>> CreateWishlist(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.CreateWishlistAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetWishlist), new { userId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wishlist for user: {UserId}", userId);
            return StatusCode(500, Result<WishlistDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("users/{userId:guid}/wishlist/items")]
    public async Task<ActionResult<Result<List<WishlistItemDto>>>> GetWishlistItems(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.GetWishlistItemsAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist items for user: {UserId}", userId);
            return StatusCode(500, Result<List<WishlistItemDto>>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/wishlist/items")]
    public async Task<ActionResult<Result<WishlistItemDto>>> AddItemToWishlist(
        Guid userId, 
        [FromBody] AddToWishlistRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.AddItemToWishlistAsync(userId, request.ProductId, request.VariantId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetWishlistItems), new { userId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to wishlist for user: {UserId}", userId);
            return StatusCode(500, Result<WishlistItemDto>.Failure("Internal server error"));
        }
    }

    [HttpDelete("users/{userId:guid}/wishlist/items/{itemId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemFromWishlist(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RemoveItemFromWishlistAsync(userId, itemId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing item from wishlist: {ItemId} for user: {UserId}", itemId, userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("users/{userId:guid}/wishlist/products/{productId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemByProductFromWishlist(
        Guid userId, 
        Guid productId, 
        [FromQuery] Guid? variantId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RemoveItemByProductFromWishlistAsync(userId, productId, variantId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing product from wishlist: {ProductId} for user: {UserId}", productId, userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpGet("users/{userId:guid}/wishlist/products/{productId:guid}/check")]
    public async Task<ActionResult<Result<bool>>> IsProductInWishlist(
        Guid userId, 
        Guid productId, 
        [FromQuery] Guid? variantId = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.IsProductInWishlistAsync(userId, productId, variantId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if product is in wishlist: {ProductId} for user: {UserId}", productId, userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/wishlist/items/{itemId:guid}/move-to-cart")]
    public async Task<ActionResult<Result<bool>>> MoveItemToCart(
        Guid userId, 
        Guid itemId, 
        [FromBody] MoveToCartRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.MoveItemToCartAsync(userId, itemId, request.Quantity, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving item to cart: {ItemId} for user: {UserId}", itemId, userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion

    #region Cart Utilities

    [HttpPost("users/{userId:guid}/merge")]
    public async Task<ActionResult<Result<bool>>> MergeCarts(
        Guid userId, 
        [FromBody] MergeCartsRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.MergeCartsAsync(userId, request.GuestCartId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error merging carts for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/coupon")]
    public async Task<ActionResult<Result<bool>>> ApplyCoupon(
        Guid userId, 
        [FromBody] ApplyCouponRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.ApplyCouponAsync(userId, request.CouponCode, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying coupon for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("users/{userId:guid}/coupon")]
    public async Task<ActionResult<Result<bool>>> RemoveCoupon(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RemoveCouponAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing coupon for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/save-for-later")]
    public async Task<ActionResult<Result<bool>>> SaveCartForLater(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.SaveCartForLaterAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving cart for later for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPost("users/{userId:guid}/restore")]
    public async Task<ActionResult<Result<bool>>> RestoreSavedCart(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _cartService.RestoreSavedCartAsync(userId, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error restoring saved cart for user: {UserId}", userId);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion
}

// Request DTOs
public class UpdateQuantityRequest
{
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}

public class RemoveItemRequest
{
    public Guid UserId { get; set; }
}

public class AddToWishlistRequest
{
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
}

public class MoveToCartRequest
{
    public int Quantity { get; set; } = 1;
}

public class MergeCartsRequest
{
    public Guid GuestCartId { get; set; }
}

public class ApplyCouponRequest
{
    public string CouponCode { get; set; } = string.Empty;
}
