using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;
using GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetPurchaseOrdersPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetPurchaseOrdersPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{purchaseOrderId}")]
    public async Task<IActionResult> GetPurchaseOrderById([FromRoute] Guid purchaseOrderId)
    {
        var result = await mediator.Send(new GetPurchaseOrderByIdQuery(purchaseOrderId));
        return Ok(result);
    }

    [HttpGet("supplier/{supplierId}/paging")]
    public async Task<IActionResult> GetPurchaseOrdersBySupplier([FromRoute] Guid supplierId, [FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetPurchaseOrdersBySupplierQuery(supplierId, request));
        return Ok(result);
    }

    [HttpGet("status/{status}/paging")]
    public async Task<IActionResult> GetPurchaseOrdersByStatus([FromRoute] short status, [FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetPurchaseOrdersByStatusQuery(status, request));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{purchaseOrderId}/status")]
    public async Task<IActionResult> UpdatePurchaseOrderStatus([FromRoute] Guid purchaseOrderId, [FromBody] UpdatePurchaseOrderStatusCommand command)
    {
        if (purchaseOrderId != command.PurchaseOrderId)
        {
            return BadRequest("Purchase Order ID mismatch");
        }
        
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{purchaseOrderId}")]
    public async Task<IActionResult> DeletePurchaseOrder([FromRoute] Guid purchaseOrderId)
    {
        var result = await mediator.Send(new DeletePurchaseOrderCommand(purchaseOrderId));
        return Ok(result);
    }
}

