using MediatR;
using Microsoft.AspNetCore.Mvc;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Queries;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Common;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController(IMediator mediator, IAzureBlobStorageService blobStorageService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<ActionResult<Result<bool>>> CreateVariant([FromBody] CreateProductVariantRequest request)
    {
        var result = await mediator.Send(new CreateProductVariantCommand(request));
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Result<bool>>> UpdateVariant([FromQuery] Guid variantId, [FromBody] UpdateProductVariantRequest request)
    {
        var result = await mediator.Send(new UpdateProductVariantCommand(variantId, request));
        return Ok(result);
    }

    [HttpPost("create-with-file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<bool>>> CreateVariantWithFile(
        [FromForm] Guid productId,
        [FromForm] string sku,
        [FromForm] string name,
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
        var request = new CreateProductVariantRequest
        {
            ProductId = productId,
            Sku = sku,
            Name = name,
            Price = price,
            DiscountPrice = discountPrice,
            StockQuantity = stockQuantity,
            MinStockLevel = minStockLevel,
            Weight = weight,
            Dimensions = dimensions,
            Status = status,
            ImageFile = imageFile != null && imageFile.Length > 0
                ? new FileUploadDto
                {
                    Content = await ToBytesAsync(imageFile, cancellationToken),
                    FileName = imageFile.FileName,
                    ContentType = imageFile.ContentType
                }
                : null
        };

        var result = await mediator.Send(new CreateProductVariantCommand(request));
        return Ok(result);
    }

    private static async Task<byte[]> ToBytesAsync(IFormFile file, CancellationToken cancellationToken)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }

    [HttpPut("{variantId}/upload-image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result<bool>>> UploadVariantImage([FromRoute] Guid variantId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Result<bool>.Failure("No file uploaded"));
        }
        var updateRequest = new UpdateProductVariantRequest
        {
            Sku = string.Empty,
            Name = null,
            Price = 0,
            DiscountPrice = null,
            StockQuantity = 0,
            MinStockLevel = 0,
            Weight = null,
            Dimensions = null,
            Status = 1,
            ImageFile = new FileUploadDto
            {
                Content = await ToBytesAsync(file, cancellationToken),
                FileName = file.FileName,
                ContentType = file.ContentType
            }
        };
        var result = await mediator.Send(new UpdateProductVariantCommand(variantId, updateRequest));
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
