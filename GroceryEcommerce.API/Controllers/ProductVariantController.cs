using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Features.ProductVariant.Commands;
using GroceryEcommerce.Application.Features.ProductVariant.Queries;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductVariantResponse>>> CreateVariant([FromBody] CreateProductVariantCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductVariantResponse>>> UpdateVariant([FromBody] UpdateProductVariantCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{variantId}")]
    public async Task<ActionResult<Result<bool>>> DeleteVariant([FromRoute] Guid variantId)
    {
        var command = new DeleteProductVariantCommand(variantId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-stock")]
    public async Task<ActionResult<Result<bool>>> UpdateVariantStock([FromBody] UpdateProductVariantStockCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("by-id/{variantId}")]
    public async Task<ActionResult<Result<ProductVariantDto>>> GetById([FromRoute] Guid variantId)
    {
        var query = new GetProductVariantByIdQuery(variantId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<Result<PagedResult<ProductVariantDto>>>> GetByProduct(
        [FromRoute] Guid productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null)
    {
        var query = new GetProductVariantsByProductQuery(productId, page, pageSize, sortBy, sortDirection);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<Result<PagedResult<ProductVariantDto>>>> GetLowStock(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        [FromQuery] int threshold = 10)
    {
        var query = new GetLowStockVariantsQuery(page, pageSize, sortBy, sortDirection, threshold);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
