using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductVariant.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductVariant.Handlers;

public class UpdateProductVariantStockCommandHandler(
    IProductVariantRepository repository,
    ILogger<UpdateProductVariantStockCommandHandler> logger
) : IRequestHandler<UpdateProductVariantStockCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProductVariantStockCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating stock for variant {VariantId} to {Quantity}", request.VariantId, request.Quantity);

        var result = await repository.UpdateStockAsync(request.VariantId, request.Quantity, cancellationToken);
        if (!result.IsSuccess || !result.Data)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update variant stock");
        }

        return Result<bool>.Success(true);
    }
}
