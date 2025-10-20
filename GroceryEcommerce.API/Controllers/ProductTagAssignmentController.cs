using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Features.ProductTagAssignment.Commands;
using GroceryEcommerce.Application.Features.ProductTagAssignment.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTagAssignmentController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AssignTagToProduct([FromBody] AssignTagToProductCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{productId}/{tagId}")]
    public async Task<IActionResult> RemoveTagFromProduct([FromRoute] Guid productId, [FromRoute] Guid tagId)
    {
        var result = await mediator.Send(new RemoveTagFromProductCommand(productId, tagId));
        return Ok(result);
    }

    [HttpDelete("product/{productId}")]
    public async Task<IActionResult> RemoveAllTagsFromProduct([FromRoute] Guid productId)
    {
        var result = await mediator.Send(new RemoveAllTagsFromProductCommand(productId));
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct([FromRoute] Guid productId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
    {
        var result = await mediator.Send(new GetProductTagAssignmentsByProductQuery(productId, page, pageSize, sortBy, sortDirection));
        return Ok(result);
    }

    [HttpGet("tag/{tagId}")]
    public async Task<IActionResult> GetByTag([FromRoute] Guid tagId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortBy = null, [FromQuery] string? sortDirection = null)
    {
        var result = await mediator.Send(new GetProductTagAssignmentsByTagQuery(tagId, page, pageSize, sortBy, sortDirection));
        return Ok(result);
    }
}

