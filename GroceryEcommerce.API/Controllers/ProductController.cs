using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    [Consumes("multipart/form-data")] 
    public async Task<ActionResult<Result<CreateProductResponse>>> Create([FromForm] CreateProductWithFilesCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("create-with-urls")]
    public async Task<ActionResult<Result<CreateProductResponse>>> CreateWithUrls([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<ProductBaseResponse>>>> GetProductsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetProductsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<Result<GetProductByIdResponse>>> GetById([FromRoute] Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("sku/{sku}")]
    public async Task<ActionResult<Result<GetProductBySkuResponse>>> GetBySku([FromRoute] string sku)
    {
        var query = new GetProductBySkuQuery(sku);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<Result<GetProductBySlugResponse>>> GetBySlug([FromRoute] string slug)
    {
        var query = new GetProductBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("update")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<UpdateProductResponse>>> UpdateWithFiles([FromForm] UpdateProductWithFilesCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-simple")]
    public async Task<ActionResult<Result<UpdateProductResponse>>> Update([FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{productId}")]
    public async Task<ActionResult<Result<bool>>> Delete([FromRoute] Guid productId)
    {
        var command = new DeleteProductCommand(productId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-status")]
    public async Task<ActionResult<Result<bool>>> UpdateStatus([FromBody] UpdateProductStatusCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-stock")]
    public async Task<ActionResult<Result<bool>>> UpdateStock([FromBody] UpdateProductStockCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<Result<GetFeaturedProductsResponse>>> GetFeatured(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] string? sortDirection = "Asc")
    {
        var query = new GetFeaturedProductsQuery(page, pageSize, sortBy, sortDirection);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<Result<GetActiveProductsResponse>>> GetActive(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] string? sortDirection = "Asc")
    {
        var query = new GetActiveProductsQuery(page, pageSize, sortBy, sortDirection);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<Result<SearchProductsResponse>>> Search(
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? brandId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? isFeatured = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] List<Guid>? tagIds = null,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] string? sortDirection = "Asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new SearchProductsQuery(searchTerm, categoryId, brandId, minPrice, maxPrice, isFeatured, isActive, tagIds, sortBy, sortDirection, page, pageSize);
        var result = await mediator.Send(query);
        return Ok(result);
    }
}