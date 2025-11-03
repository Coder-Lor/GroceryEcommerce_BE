using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class UpdateProductVariantCommandHandler(
    IProductVariantRepository repository,
    IAzureBlobStorageService blobStorageService,
    IMapper mapper,
    ILogger<UpdateProductVariantCommandHandler> logger
) : IRequestHandler<UpdateProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        try {
            logger.LogInformation("Updating product variant {VariantId}", request.VariantId);

            var existing = await repository.GetByIdAsync(request.VariantId, cancellationToken);
            if (!existing.IsSuccess || existing.Data is null)
            {
                return Result<bool>.Failure("Product variant not found");
            }

            var updateReq = request.Request;

            // Upload image in handler if provided
            if (updateReq.ImageFile != null && updateReq.ImageFile.Content.Length > 0)
            {
                using var stream = new MemoryStream(updateReq.ImageFile.Content);
                var imageUrl = await blobStorageService.UploadImageAsync(stream, updateReq.ImageFile.FileName, updateReq.ImageFile.ContentType, cancellationToken);
                updateReq.ImageUrl = imageUrl;
            }

            existing.Data.Sku = updateReq.Sku;
            existing.Data.Name = updateReq.Name;
            existing.Data.Price = updateReq.Price;
            existing.Data.DiscountPrice = updateReq.DiscountPrice;
            existing.Data.StockQuantity = updateReq.StockQuantity;
            existing.Data.Weight = updateReq.Weight;
            existing.Data.ImageUrl = updateReq.ImageUrl;
            existing.Data.Status = updateReq.Status;
            existing.Data.UpdatedAt = DateTime.UtcNow;

            var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update product variant");
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex){
            logger.LogError(ex, "Error updating product variant");
            return Result<bool>.Failure("An error occurred while updating product variant.", ex.Message);
        }
    }
}
