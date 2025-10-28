using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class DeleteProductCommandHandler(
    IProductRepository repository,
    ILogger<DeleteProductCommandHandler> logger
) : IRequestHandler<DeleteProductCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling DeleteProductCommand for product: {ProductId}", request.ProductId);

        var result = await repository.DeleteAsync(request.ProductId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to delete product: {ProductId}", request.ProductId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete product.");
        }

        logger.LogInformation("Product deleted successfully: {ProductId}", request.ProductId);
        return Result<bool>.Success(true);
    }
}
