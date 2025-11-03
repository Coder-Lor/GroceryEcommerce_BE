using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class DeleteProductVariantCommandHandler(
    IProductVariantRepository repository,
    IAzureBlobStorageService blobStorageService,
    ILogger<DeleteProductVariantCommandHandler> logger
) : IRequestHandler<DeleteProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product variant {VariantId}", request.VariantId);

        var variantResult = await repository.GetByIdAsync(request.VariantId, cancellationToken);
        if (!variantResult.IsSuccess || variantResult.Data is null)
        {
            return Result<bool>.Failure("Product variant not found");
        }

        // Delete associated image in blob storage if exists
        var imageUrl = variantResult.Data.ImageUrl;
        if (!string.IsNullOrWhiteSpace(imageUrl))
        {
            var blobName = ExtractBlobName(imageUrl);
            if (!string.IsNullOrWhiteSpace(blobName))
            {
                var deleted = await blobStorageService.DeleteImageAsync(blobName, cancellationToken);
                if (!deleted)
                {
                    logger.LogWarning("Failed to delete blob image for variant {VariantId}: {BlobName}", request.VariantId, blobName);
                }
            }
        }

        var del = await repository.DeleteAsync(request.VariantId, cancellationToken);
        if (!del.IsSuccess || !del.Data)
        {
            return Result<bool>.Failure(del.ErrorMessage ?? "Failed to delete product variant");
        }

        return Result<bool>.Success(true);
    }

    private static string ExtractBlobName(string urlOrName)
    {
        // If it's already a blob name (no slash), return as is
        if (!urlOrName.Contains('/')) return urlOrName;
        var lastSlash = urlOrName.LastIndexOf('/');
        if (lastSlash < 0 || lastSlash == urlOrName.Length - 1) return urlOrName;
        // Strip query string if present
        var pathPart = urlOrName.Substring(lastSlash + 1);
        var qIndex = pathPart.IndexOf('?');
        return qIndex >= 0 ? pathPart.Substring(0, qIndex) : pathPart;
    }
}
