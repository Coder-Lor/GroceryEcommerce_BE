using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetWarehousesPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetWarehousesPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("active/paging")]
    public async Task<IActionResult> GetActiveWarehousesPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetActiveWarehousesPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{warehouseId}")]
    public async Task<IActionResult> GetWarehouseById([FromRoute] Guid warehouseId)
    {
        var result = await mediator.Send(new GetWarehouseByIdQuery(warehouseId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{warehouseId}")]
    public async Task<IActionResult> UpdateWarehouse([FromRoute] Guid warehouseId, [FromBody] UpdateWarehouseCommand command)
    {
        if (warehouseId != command.WarehouseId)
        {
            return BadRequest("Warehouse ID mismatch");
        }
        
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{warehouseId}")]
    public async Task<IActionResult> DeleteWarehouse([FromRoute] Guid warehouseId)
    {
        var result = await mediator.Send(new DeleteWarehouseCommand(warehouseId));
        return Ok(result);
    }
}

