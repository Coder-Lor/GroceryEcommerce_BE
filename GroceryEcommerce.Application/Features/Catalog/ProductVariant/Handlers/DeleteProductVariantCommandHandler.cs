using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class DeleteProductVariantCommandHandler(
    IProductVariantRepository repository,
    ILogger<DeleteProductVariantCommandHandler> logger
) : IRequestHandler<DeleteProductVariantCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product variant {VariantId}", request.VariantId);

        var exists = await repository.ExistsAsync(request.VariantId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            return Result<bool>.Failure("Product variant not found");
        }

        var del = await repository.DeleteAsync(request.VariantId, cancellationToken);
        if (!del.IsSuccess || !del.Data)
        {
            return Result<bool>.Failure(del.ErrorMessage ?? "Failed to delete product variant");
        }

        return Result<bool>.Success(true);
    }
}
