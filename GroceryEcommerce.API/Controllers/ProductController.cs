using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Commands;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IMediator mediator) : ControllerBase {

    [HttpGet("paging")]
    public async Task<ActionResult<Result<PagedResult<ProductBaseResponse>>>> GetProductsPaging([FromQuery] PagedRequest request)
    {
        var query = new GetProductsPagingQuery(request);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetFeaturedProducts()
    {
        var query = new GetFeaturedProductsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetActiveProducts()
    {
        var query = new GetActiveProductsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetLowStockProducts()
    {
        var query = new GetLowStockProductsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-category/{categoryId}")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetProductsByCategory([FromRoute] Guid categoryId)
    {
        var query = new GetProductsByCategoryQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-brand/{brandId}")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetProductsByBrand([FromRoute] Guid brandId)
    {
        var query = new GetProductsByBrandQuery(brandId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-price-range")]
    public async Task<ActionResult<Result<List<ProductBaseResponse>>>> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var query = new GetProductsByPriceRangeQuery(minPrice, maxPrice);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-sku/{sku}")]
    public async Task<ActionResult<Result<bool>>> CheckProductExistsBySku([FromRoute] string sku)
    {
        var query = new CheckProductExistsBySkuQuery(sku);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-id/{productId}")]
    public async Task<ActionResult<Result<bool>>> CheckProductExistsById([FromRoute] Guid productId)
    {
        var query = new CheckProductExistsByIdQuery(productId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-slug/{slug}")]
    public async Task<ActionResult<Result<bool>>> CheckProductExistsBySlug([FromRoute] string slug)
    {
        var query = new CheckProductExistsBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("exists-by-category-id/{categoryId}")]
    public async Task<ActionResult<Result<bool>>> CheckProductExistsByCategoryId([FromRoute] Guid categoryId)
    {
        var query = new CheckProductExistsByCategoryIdQuery(categoryId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<Result<CreateProductResponse>>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<UpdateProductResponse>>> UpdateProduct([FromBody] UpdateProductCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-status")]
    public async Task<ActionResult<Result<UpdateProductStatusResponse>>> UpdateProductStatus([FromBody] UpdateProductStatusCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update-stock")]
    public async Task<ActionResult<Result<UpdateProductStockResponse>>> UpdateProductStock([FromBody] UpdateProductStockCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("delete/{productId}")]
    public async Task<ActionResult<Result<bool>>> DeleteProduct([FromRoute] Guid productId)
    {
        var command = new DeleteProductCommand(productId);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("by-id/{productId}")]
    public async Task<ActionResult<Result<GetProductByIdResponse>>> GetProductById([FromRoute] Guid productId)
    {
        var query = new GetProductByIdQuery(productId);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-sku/{sku}")]
    public async Task<ActionResult<Result<GetProductBySkuResponse>>> GetProductBySku([FromRoute] string sku)
    {
        var query = new GetProductBySkuQuery(sku);
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<Result<GetProductBySlugResponse>>> GetProductBySlug([FromRoute] string slug)
    {
        var query = new GetProductBySlugQuery(slug);
        var result = await mediator.Send(query);
        return Ok(result);
    }    
}