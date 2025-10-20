using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Commands;
using GroceryEcommerce.Application.Features.ProductImage.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductImageController(IMediator mediator) : ControllerBase
{
    [HttpGet("by-id/{imageId}")]
    public async Task<ActionResult<Result<ProductImageDto>>> GetById([FromRoute] Guid imageId)
    {
        var query = new GetProductImageByIdQuery(imageId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-product/{productId}")]
    public async Task<ActionResult<Result<PagedResult<ProductImageDto>>>> GetByProduct([FromRoute] Guid productId, [FromQuery] PagedRequest request)
    {
        var sortDirStr = request.SortDirection == SortDirection.Descending ? "Desc" : "Asc";
        var query = new GetProductImagesPagingQuery(productId, request.Page, request.PageSize, request.SortBy, sortDirStr);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductImageResponse>>> Create([FromBody] CreateProductImageCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductImageResponse>>> Update([FromBody] UpdateProductImageCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("set-primary/{imageId}")]
    public async Task<ActionResult<Result<bool>>> SetPrimary([FromRoute] Guid imageId)
    {
        var command = new SetPrimaryProductImageCommand(imageId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{imageId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid imageId)
    {
        var command = new DeleteProductImageCommand(imageId);
        var result = await mediator.Send(command);
        return Ok(result);
    }
}

