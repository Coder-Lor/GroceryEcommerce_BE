using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderShipmentController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderShipmentDto>>>> GetOrderShipmentsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrderShipmentsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{shipmentId}")]
    public async Task<ActionResult<Result<OrderShipmentDto>>> GetById([FromRoute] Guid shipmentId)
    {
        var query = new GetOrderShipmentByIdQuery(shipmentId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<Result<List<OrderShipmentDto>>>> GetByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetOrderShipmentsByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderShipmentDto>>> Create([FromBody] CreateOrderShipmentRequest request)
    {
        var command = new CreateOrderShipmentCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update/{shipmentId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid shipmentId,
        [FromBody] UpdateOrderShipmentRequest request)
    {
        var command = new UpdateOrderShipmentCommand(shipmentId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-status/{shipmentId}")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(
        [FromRoute] Guid shipmentId,
        [FromBody] UpdateShipmentStatusRequest request)
    {
        var command = new UpdateShipmentStatusCommand(shipmentId, request.Status);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{shipmentId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid shipmentId)
    {
        var command = new DeleteOrderShipmentCommand(shipmentId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

public record UpdateShipmentStatusRequest(short Status);

