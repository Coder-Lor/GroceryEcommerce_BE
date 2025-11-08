using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Commands;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockMovementController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetStockMovementsPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetStockMovementsPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{movementId}")]
    public async Task<IActionResult> GetStockMovementById([FromRoute] Guid movementId)
    {
        var result = await mediator.Send(new GetStockMovementByIdQuery(movementId));
        return Ok(result);
    }

    [HttpGet("product/{productId}/paging")]
    public async Task<IActionResult> GetStockMovementsByProduct([FromRoute] Guid productId, [FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetStockMovementsByProductQuery(productId, request));
        return Ok(result);
    }

    [HttpGet("current-stock")]
    public async Task<IActionResult> GetCurrentStock([FromQuery] Guid productId, [FromQuery] Guid? warehouseId)
    {
        var result = await mediator.Send(new GetCurrentStockQuery(productId, warehouseId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStockMovement([FromBody] CreateStockMovementCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

