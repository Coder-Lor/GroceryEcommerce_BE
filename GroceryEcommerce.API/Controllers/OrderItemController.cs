using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderItemController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderItemDto>>>> GetOrderItemsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrderItemsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{orderItemId}")]
    public async Task<ActionResult<Result<OrderItemDto>>> GetById([FromRoute] Guid orderItemId)
    {
        var query = new GetOrderItemByIdQuery(orderItemId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<Result<List<OrderItemDto>>>> GetByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetOrderItemsByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderItemDto>>> Create([FromBody] CreateOrderItemRequest request)
    {
        var command = new CreateOrderItemCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update/{orderItemId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid orderItemId,
        [FromBody] UpdateOrderItemRequest request)
    {
        var command = new UpdateOrderItemCommand(orderItemId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{orderItemId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid orderItemId)
    {
        var command = new DeleteOrderItemCommand(orderItemId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

