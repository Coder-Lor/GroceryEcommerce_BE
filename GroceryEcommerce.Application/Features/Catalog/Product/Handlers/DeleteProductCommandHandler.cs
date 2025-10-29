using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class DeleteProductCommandHandler(
    IProductRepository repository,
    IProductImageRepository productImageRepository,
    IAzureBlobStorageService blobStorageService,
    ILogger<DeleteProductCommandHandler> logger,
    IUnitOfWorkService unitOfWorkService)
    : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling DeleteProductCommand for product: {ProductId}", request.ProductId);

            // Get product to check if it exists and get images
            var productResult = await repository.GetByIdAsync(request.ProductId, cancellationToken);
            if (!productResult.IsSuccess || productResult.Data == null)
            {
                logger.LogWarning("Product not found: {ProductId}", request.ProductId);
                return Result<bool>.Failure("Product not found.");
            }

            await unitOfWorkService.BeginTransactionAsync(cancellationToken);

            try
            {
                // Get all product images
                var pagedRequest = new PagedRequest { Page = 1, PageSize = 1000 };
                var imagesResult = await productImageRepository.GetByProductIdAsync(pagedRequest, request.ProductId, cancellationToken);
                
                if (imagesResult.IsSuccess && imagesResult.Data != null)
                {
                    // Delete images from blob storage and database
                    foreach (var image in imagesResult.Data.Items)
                    {
                        try
                        {
                            // Extract blob name from URL
                            var blobName = ExtractBlobNameFromUrl(image.ImageUrl);
                            if (!string.IsNullOrEmpty(blobName))
                            {
                                // Delete from blob storage
                                await blobStorageService.DeleteImageAsync(blobName, cancellationToken);
                            }

                            // Delete from database
                            await productImageRepository.DeleteAsync(image.ImageId, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error deleting product image {ImageId}", image.ImageId);
                            // Continue with other images even if one fails
                        }
                    }
                }

                // Delete product
                var deleteResult = await repository.DeleteAsync(request.ProductId, cancellationToken);
                if (!deleteResult.IsSuccess)
                {
                    logger.LogWarning("Failed to delete product: {ProductId}", request.ProductId);
                    await unitOfWorkService.RollbackAsync(cancellationToken);
                    return Result<bool>.Failure(deleteResult.ErrorMessage ?? "Failed to delete product.");
                }

                await unitOfWorkService.CommitAsync(cancellationToken);

                logger.LogInformation("Product deleted successfully: {ProductId}", request.ProductId);
                return Result<bool>.Success(true);
            }
            catch
            {
                await unitOfWorkService.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product: {ProductId}", request.ProductId);
            return Result<bool>.Failure("An error occurred while deleting the product.");
        }
    }

    private string? ExtractBlobNameFromUrl(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return null;

        try
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath;
            
            // Extract blob name from path (format: /container/{blobName} or /{blobName})
            var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                // Return the last part as blob name
                return parts[^1];
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to extract blob name from URL: {ImageUrl}", imageUrl);
        }

        return null;
    }
}
