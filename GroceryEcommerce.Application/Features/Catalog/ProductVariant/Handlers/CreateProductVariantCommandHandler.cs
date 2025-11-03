using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class CreateProductVariantCommandHandler(
    IProductVariantRepository repository,
    IAzureBlobStorageService blobStorageService,
    IMapper mapper,
    ILogger<CreateProductVariantCommandHandler> logger
) : IRequestHandler<CreateProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product variant for product {ProductId}", request.ProductId);

        var existingVariant = await repository.GetBySkuAsync(request.Sku, cancellationToken);
        if (existingVariant.IsSuccess && existingVariant.Data != null)
        {
            return Result<bool>.Failure("Variant with this SKU already exists");
        }

        var createReq = request.Request;

        // Upload image in handler if provided
        if (createReq.ImageFile != null && createReq.ImageFile.Content.Length > 0)
        {
            using var stream = new MemoryStream(createReq.ImageFile.Content);
            var imageUrl = await blobStorageService.UploadImageAsync(stream, createReq.ImageFile.FileName, createReq.ImageFile.ContentType, cancellationToken);
            createReq.ImageUrl = imageUrl;
        }

        var entity = mapper.Map<Domain.Entities.Catalog.ProductVariant>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to create product variant");
        }

        return Result<bool>.Success(true);
    }
}
