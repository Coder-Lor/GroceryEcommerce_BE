using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;
using GroceryEcommerce.Application.Features.Cart.WishlistItem.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistController(IMediator mediator) : ControllerBase
{
    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<Result<WishlistDto>>> GetWishlist(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetDefaultWishlistByUserIdQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("users/{userId:guid}/items")]
    public async Task<ActionResult<Result<bool>>> AddItem(
        Guid userId,
        [FromBody] AddToWishlistRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new AddWishlistItemCommand(userId, request.ProductId, request.VariantId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete("items/{itemId:guid}")]
    public async Task<ActionResult<Result<bool>>> RemoveItem(Guid itemId, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new RemoveWishlistItemCommand(itemId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("wishlists/{wishlistId:guid}/items")]
    public async Task<ActionResult<Result<PagedResult<WishlistItemDto>>>> GetWishlistItems(
        Guid wishlistId,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetWishlistItemsPagedQuery(request, wishlistId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}/items")]
    public async Task<ActionResult<Result<PagedResult<WishlistItemDto>>>> GetUserWishlistItems(
        Guid userId,
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetWishlistItemsByUserPagedQuery(request, userId), cancellationToken);
        return Ok(result);
    }
}

// request DTOs moved to Contracts/Requests

