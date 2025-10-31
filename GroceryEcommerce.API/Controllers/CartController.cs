using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(IMediator mediator) : ControllerBase
{
    #region Shopping Cart

    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<Result<ShoppingCartDto>>> GetShoppingCart(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetShoppingCartByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("users/{userId:guid}")]
    public async Task<ActionResult<Result<bool>>> ClearShoppingCart(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ClearShoppingCartCommand(userId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("users/{userId:guid}/summary")]
    public async Task<ActionResult<Result<CartSummaryDto>>> GetCartSummary(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetCartSummaryQuery(userId), cancellationToken);
        return Ok(result);
    }

    #endregion

    #region Shopping Cart Items

    [HttpPost("items")]
    public async Task<ActionResult<Result<bool>>> AddItemToCart([FromBody] AddToCartRequest request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new AddShoppingCartItemCommand(
            request.ProductId,
            request.ProductVariantId,
            request.Quantity
        ), cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("items/{itemId:guid}/quantity")]
    public async Task<ActionResult<Result<bool>>> UpdateCartItemQuantity(
        Guid itemId, 
        [FromBody] UpdateQuantityRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new UpdateShoppingCartItemQuantityCommand(itemId, request.Quantity), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemFromCart(Guid itemId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new RemoveShoppingCartItemCommand(itemId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion

    #region Wishlist

    [HttpGet("users/{userId:guid}/wishlist")]
    public async Task<ActionResult<Result<WishlistDto>>> GetWishlist(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetDefaultWishlistByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}/wishlist/items")]
    public async Task<ActionResult<Result<PagedResult<WishlistItemDto>>>> GetWishlistItems(
        Guid userId,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = new GetWishlistItemsPagedQuery(request, Guid.Empty); // Will need wishlistId
        var result = await mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/wishlist/items")]
    public async Task<ActionResult<Result<bool>>> AddItemToWishlist(
        Guid userId, 
        [FromBody] AddToWishlistRequest request, 
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new AddWishlistItemCommand(userId, request.ProductId, request.VariantId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("users/{userId:guid}/wishlist/items/{itemId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItemFromWishlist(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new RemoveWishlistItemCommand(itemId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    #endregion
}

// request DTOs moved to Contracts/Requests
