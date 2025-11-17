using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderDto>>> Create([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderDto>>>> GetOrdersPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrdersPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<Result<OrderDetailDto>>> GetById([FromRoute] Guid orderId)
    {
        var query = new GetOrderByIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<Result<PagedResult<OrderDto>>>> GetByUserId(
        [FromRoute] Guid userId,
        [FromQuery] PagedRequest request)
    {
        var query = new GetOrdersByUserIdQuery(userId, request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("update/{orderId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderRequest request)
    {
        var command = new UpdateOrderCommand(orderId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-status/{orderId}")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(
        [FromRoute] Guid orderId,
        [FromBody] UpdateOrderStatusRequest request)
    {
        var command = new UpdateOrderStatusCommand(orderId, request.Status);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid orderId)
    {
        var command = new DeleteOrderCommand(orderId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

public record UpdateOrderStatusRequest(short Status);

