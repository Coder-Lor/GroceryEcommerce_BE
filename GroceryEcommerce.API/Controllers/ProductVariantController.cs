using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Queries;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController(IMediator mediator, IAzureBlobStorageService blobStorageService) : ControllerBase
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

    [HttpPost("create-with-file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<CreateProductVariantResponse>>> CreateVariantWithFile(
        [FromForm] Guid productId,
        [FromForm] string sku,
        [FromForm] string variantName,
        [FromForm] decimal price,
        [FromForm] int stockQuantity,
        [FromForm] int minStockLevel,
        [FromForm] short status,
        [FromForm] decimal? discountPrice,
        [FromForm] decimal? weight,
        [FromForm] string? dimensions,
        [FromForm] IFormFile? imageFile,
        CancellationToken cancellationToken)
    {
        string? imageUrl = null;
        if (imageFile != null && imageFile.Length > 0)
        {
            using var stream = imageFile.OpenReadStream();
            imageUrl = await blobStorageService.UploadImageAsync(stream, imageFile.FileName, imageFile.ContentType, cancellationToken);
        }

        var command = new CreateProductVariantCommand(productId, sku, variantName, price, discountPrice, stockQuantity, minStockLevel, weight, dimensions, imageUrl, status);
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{variantId}/upload-image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<string>>> UploadVariantImage([FromRoute] Guid variantId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Result<string>.Failure("No file uploaded"));
        }
        using var stream = file.OpenReadStream();
        var imageUrl = await blobStorageService.UploadImageAsync(stream, file.FileName, file.ContentType, cancellationToken);
        // Endpoint trả URL; FE gọi UpdateVariant với ImageUrl
        return Ok(Result<string>.Success(imageUrl));
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
