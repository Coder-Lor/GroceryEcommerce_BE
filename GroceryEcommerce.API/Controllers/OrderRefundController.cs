using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderRefundController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderRefundDto>>>> GetOrderRefundsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrderRefundsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{refundId}")]
    public async Task<ActionResult<Result<OrderRefundDto>>> GetById([FromRoute] Guid refundId)
    {
        var query = new GetOrderRefundByIdQuery(refundId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<Result<List<OrderRefundDto>>>> GetByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetOrderRefundsByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderRefundDto>>> Create([FromBody] CreateOrderRefundRequest request)
    {
        var command = new CreateOrderRefundCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update/{refundId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid refundId,
        [FromBody] UpdateOrderRefundRequest request)
    {
        var command = new UpdateOrderRefundCommand(refundId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("process/{refundId}")]
    public async Task<ActionResult<Result<bool>>> ProcessRefund(
        [FromRoute] Guid refundId,
        [FromBody] ProcessRefundRequest request)
    {
        var command = new ProcessRefundCommand(refundId, request.ProcessedBy);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{refundId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid refundId)
    {
        var command = new DeleteOrderRefundCommand(refundId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

public record ProcessRefundRequest(Guid ProcessedBy);

