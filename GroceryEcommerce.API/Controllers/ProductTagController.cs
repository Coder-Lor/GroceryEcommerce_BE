using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductTag.Commands;
using GroceryEcommerce.Application.Features.ProductTag.Queries;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTagController(IMediator mediator) : ControllerBase
{
    [HttpGet("paging")]
    public async Task<IActionResult> GetProductTagsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetProductTagsPagingQuery(request.Page, request.PageSize, request.SortBy, request.SortDirection == SortDirection.Descending ? "Desc" : "Asc", request.Search);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{tagId}")]
    public async Task<IActionResult> GetProductTagById([FromRoute] Guid tagId)
    {
        var result = await mediator.Send(new GetProductTagByIdQuery(tagId));
        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetProductTagByName([FromRoute] string name)
    {
        var result = await mediator.Send(new GetProductTagByNameQuery(name));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductTag([FromBody] CreateProductTagCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpPut("{tagId}")]
    public async Task<IActionResult> UpdateProductTag([FromRoute] Guid tagId, [FromBody] UpdateProductTagCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result);
    }

    [HttpDelete("{tagId}")]
    public async Task<IActionResult> DeleteProductTag([FromRoute] Guid tagId)
    {
        var result = await mediator.Send(new DeleteProductTagCommand(tagId));
        return Ok(result);
    }
}

