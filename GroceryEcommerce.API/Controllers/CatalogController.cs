using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(ICatalogService catalogService, ILogger<CatalogController> logger)
    {
        _catalogService = catalogService;
        _logger = logger;
    }

    #region Categories

    [HttpGet("categories")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetCategories(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetCategoriesAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, Result<List<CategoryDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("categories/{id:guid}")]
    public async Task<ActionResult<Result<CategoryDto>>> GetCategory(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetCategoryByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with id: {CategoryId}", id);
            return StatusCode(500, Result<CategoryDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("categories/slug/{slug}")]
    public async Task<ActionResult<Result<CategoryDto>>> GetCategoryBySlug(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetCategoryBySlugAsync(slug, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category with slug: {Slug}", slug);
            return StatusCode(500, Result<CategoryDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("categories/{id:guid}/subcategories")]
    public async Task<ActionResult<Result<List<CategoryDto>>>> GetSubCategories(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetSubCategoriesAsync(id, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting subcategories for category: {CategoryId}", id);
            return StatusCode(500, Result<List<CategoryDto>>.Failure("Internal server error"));
        }
    }

    [HttpPost("categories")]
    public async Task<ActionResult<Result<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.CreateCategoryAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetCategory), new { id = result.Data?.CategoryId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return StatusCode(500, Result<CategoryDto>.Failure("Internal server error"));
        }
    }

    [HttpPut("categories/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.UpdateCategoryAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category: {CategoryId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("categories/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> DeleteCategory(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.DeleteCategoryAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category: {CategoryId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion

    #region Brands

    [HttpGet("brands")]
    public async Task<ActionResult<Result<List<BrandDto>>>> GetBrands(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetBrandsAsync(cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brands");
            return StatusCode(500, Result<List<BrandDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("brands/{id:guid}")]
    public async Task<ActionResult<Result<BrandDto>>> GetBrand(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetBrandByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brand with id: {BrandId}", id);
            return StatusCode(500, Result<BrandDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("brands/slug/{slug}")]
    public async Task<ActionResult<Result<BrandDto>>> GetBrandBySlug(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetBrandBySlugAsync(slug, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting brand with slug: {Slug}", slug);
            return StatusCode(500, Result<BrandDto>.Failure("Internal server error"));
        }
    }

    [HttpPost("brands")]
    public async Task<ActionResult<Result<BrandDto>>> CreateBrand([FromBody] CreateBrandRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.CreateBrandAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetBrand), new { id = result.Data?.BrandId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating brand");
            return StatusCode(500, Result<BrandDto>.Failure("Internal server error"));
        }
    }

    [HttpPut("brands/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> UpdateBrand(Guid id, [FromBody] UpdateBrandRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.UpdateBrandAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating brand: {BrandId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("brands/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> DeleteBrand(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.DeleteBrandAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting brand: {BrandId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion

    #region Products

    [HttpGet("products")]
    public async Task<ActionResult<Result<PagedResult<ProductDto>>>> GetProducts(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new PagedRequest { Page = page, PageSize = pageSize };
            var result = await _catalogService.GetProductsAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products");
            return StatusCode(500, Result<PagedResult<ProductDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("products/{id:guid}")]
    public async Task<ActionResult<Result<ProductDetailDto>>> GetProduct(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetProductByIdAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with id: {ProductId}", id);
            return StatusCode(500, Result<ProductDetailDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("products/slug/{slug}")]
    public async Task<ActionResult<Result<ProductDetailDto>>> GetProductBySlug(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetProductBySlugAsync(slug, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with slug: {Slug}", slug);
            return StatusCode(500, Result<ProductDetailDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("products/sku/{sku}")]
    public async Task<ActionResult<Result<ProductDto>>> GetProductBySku(string sku, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetProductBySkuAsync(sku, cancellationToken);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with sku: {Sku}", sku);
            return StatusCode(500, Result<ProductDto>.Failure("Internal server error"));
        }
    }

    [HttpGet("categories/{categoryId:guid}/products")]
    public async Task<ActionResult<Result<List<ProductDto>>>> GetProductsByCategory(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetProductsByCategoryAsync(categoryId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for category: {CategoryId}", categoryId);
            return StatusCode(500, Result<List<ProductDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("brands/{brandId:guid}/products")]
    public async Task<ActionResult<Result<List<ProductDto>>>> GetProductsByBrand(Guid brandId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetProductsByBrandAsync(brandId, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for brand: {BrandId}", brandId);
            return StatusCode(500, Result<List<ProductDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("products/featured")]
    public async Task<ActionResult<Result<List<ProductDto>>>> GetFeaturedProducts(
        [FromQuery] int limit = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.GetFeaturedProductsAsync(limit, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured products");
            return StatusCode(500, Result<List<ProductDto>>.Failure("Internal server error"));
        }
    }

    [HttpGet("products/search")]
    public async Task<ActionResult<Result<List<ProductDto>>>> SearchProducts(
        [FromQuery] string q, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest(Result<List<ProductDto>>.Failure("Search query is required"));

            var result = await _catalogService.SearchProductsAsync(q, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching products with query: {Query}", q);
            return StatusCode(500, Result<List<ProductDto>>.Failure("Internal server error"));
        }
    }

    [HttpPost("products")]
    public async Task<ActionResult<Result<ProductDto>>> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.CreateProductAsync(request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetProduct), new { id = result.Data?.ProductId }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return StatusCode(500, Result<ProductDto>.Failure("Internal server error"));
        }
    }

    [HttpPut("products/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.UpdateProductAsync(id, request, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product: {ProductId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpDelete("products/{id:guid}")]
    public async Task<ActionResult<Result<bool>>> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.DeleteProductAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product: {ProductId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPut("products/{id:guid}/stock")]
    public async Task<ActionResult<Result<bool>>> UpdateProductStock(Guid id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.UpdateProductStockAsync(id, request.Quantity, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product stock: {ProductId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    [HttpPut("products/{id:guid}/featured")]
    public async Task<ActionResult<Result<bool>>> ToggleProductFeatured(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _catalogService.ToggleProductFeaturedAsync(id, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling product featured status: {ProductId}", id);
            return StatusCode(500, Result<bool>.Failure("Internal server error"));
        }
    }

    #endregion
}

public class UpdateStockRequest
{
    public int Quantity { get; set; }
}
