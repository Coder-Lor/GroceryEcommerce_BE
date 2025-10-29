using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using GroceryEcommerce.Application.Features.Cart.AbandonedCart.Commands;
using GroceryEcommerce.Application.Features.Cart.AbandonedCart.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AbandonedCartController(IMediator mediator) : ControllerBase
{
    [HttpGet("unnotified")]
    public async Task<ActionResult<Result<PagedResult<AbandonedCartDto>>>> GetUnnotifiedCarts(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetUnnotifiedAbandonedCartsPagedQuery(request), cancellationToken);
        return Ok(result);
    }

    [HttpGet("date-range")]
    public async Task<ActionResult<Result<PagedResult<AbandonedCartDto>>>> GetByDateRange(
        [FromQuery] PagedRequest request,
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAbandonedCartsByDateRangePagedQuery(request, fromDate, toDate), cancellationToken);
        return Ok(result);
    }

    [HttpPut("notified/{abandonedCartId:guid}")]
    public async Task<ActionResult<Result<bool>>> MarkAsNotified(
        Guid abandonedCartId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new MarkAbandonedCartNotifiedCommand(abandonedCartId), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPut("notified/batch")]
    public async Task<ActionResult<Result<bool>>> MarkBatchAsNotified(
        [FromBody] MarkCartsNotifiedRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new MarkAbandonedCartsNotifiedCommand(request.AbandonedCartIds), cancellationToken);
        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }
}


