using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;
using GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetSuppliersPaging([FromQuery] PagedRequest request)
    {
        var result = await mediator.Send(new GetSuppliersPagingQuery(request));
        return Ok(result);
    }

    [HttpGet("{supplierId}")]
    public async Task<IActionResult> GetSupplierById([FromRoute] Guid supplierId)
    {
        var result = await mediator.Send(new GetSupplierByIdQuery(supplierId));
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchSuppliersByName([FromQuery] string searchTerm)
    {
        var result = await mediator.Send(new SearchSuppliersByNameQuery(searchTerm));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{supplierId}")]
    public async Task<IActionResult> UpdateSupplier([FromRoute] Guid supplierId, [FromBody] UpdateSupplierCommand command)
    {
        if (supplierId != command.SupplierId)
        {
            return BadRequest("Supplier ID mismatch");
        }
        
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{supplierId}")]
    public async Task<IActionResult> DeleteSupplier([FromRoute] Guid supplierId)
    {
        var result = await mediator.Send(new DeleteSupplierCommand(supplierId));
        return Ok(result);
    }
}

