using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductImage.Handlers;

public class DeleteProductImageCommandHandler(
    IProductImageRepository repository,
    ILogger<DeleteProductImageCommandHandler> logger
) : IRequestHandler<DeleteProductImageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product image {ImageId}", request.ImageId);

        var exists = await repository.ExistsAsync(request.ImageId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            logger.LogWarning("Product image not found: {ImageId}", request.ImageId);
            return Result<bool>.Failure("Product image not found");
        }

        var deleteResult = await repository.DeleteAsync(request.ImageId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            logger.LogWarning("Failed to delete product image {ImageId}: {Error}", request.ImageId, deleteResult.ErrorMessage);
            return Result<bool>.Failure(deleteResult.ErrorMessage ?? "Failed to delete product image");
        }

        return Result<bool>.Success(true);
    }
}


