using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderPaymentController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<OrderPaymentDto>>>> GetOrderPaymentsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetOrderPaymentsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{paymentId}")]
    public async Task<ActionResult<Result<OrderPaymentDto>>> GetById([FromRoute] Guid paymentId)
    {
        var query = new GetOrderPaymentByIdQuery(paymentId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<Result<List<OrderPaymentDto>>>> GetByOrderId([FromRoute] Guid orderId)
    {
        var query = new GetOrderPaymentsByOrderIdQuery(orderId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<OrderPaymentDto>>> Create([FromBody] CreateOrderPaymentRequest request)
    {
        var command = new CreateOrderPaymentCommand(request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update/{paymentId}")]
    public async Task<ActionResult<Result<bool>>> Update(
        [FromRoute] Guid paymentId,
        [FromBody] UpdateOrderPaymentRequest request)
    {
        var command = new UpdateOrderPaymentCommand(paymentId, request);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-status/{paymentId}")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus(
        [FromRoute] Guid paymentId,
        [FromBody] UpdatePaymentStatusRequest request)
    {
        var command = new UpdatePaymentStatusCommand(paymentId, request.Status);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{paymentId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid paymentId)
    {
        var command = new DeleteOrderPaymentCommand(paymentId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("payment-confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> PaymentConfirmation([FromBody] PaymentConfirmationRequest request)
    {
        // Process the webhook data (e.g., update payment status in the database)
        var command = new PaymentConfirmationCommand(request);
        var result = await mediator.Send(command);
        // Return a 200 OK response to acknowledge receipt of the webhook
        return Ok(result);
    }
}

public record UpdatePaymentStatusRequest(short Status);

