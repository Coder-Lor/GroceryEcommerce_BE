using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderStatusHistoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderStatusHistoryDto>>>> GetOrderStatusHistoriesPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrderStatusHistoriesPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{historyId}")]
    public async Task<ActionResult<Result<OrderStatusHistoryDto>>> GetById([FromRoute] Guid historyId)
    {
        var query = new GetOrderStatusHistoryByIdQuery(historyId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<Result<List<OrderStatusHistoryDto>>>> GetByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetOrderStatusHistoriesByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}/latest")]
    public async Task<ActionResult<Result<OrderStatusHistoryDto>>> GetLatestByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetLatestOrderStatusHistoryByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderStatusHistoryDto>>> Create([FromBody] CreateOrderStatusHistoryRequest request)
    {
        var command = new CreateOrderStatusHistoryCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("add-status-change")]
    public async Task<ActionResult<Result<bool>>> AddStatusChange([FromBody] AddStatusChangeRequest request)
    {
        var command = new AddStatusChangeCommand(
            request.OrderId,
            request.FromStatus,
            request.ToStatus,
            request.Comment,
            request.CreatedBy);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{historyId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid historyId)
    {
        var command = new DeleteOrderStatusHistoryCommand(historyId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

public record AddStatusChangeRequest(
    Guid OrderId,
    short FromStatus,
    short ToStatus,
    string? Comment,
    Guid CreatedBy);

