using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class GetCurrentStockHandler(
    IStockMovementRepository repository,
    ILogger<GetCurrentStockHandler> logger
) : IRequestHandler<GetCurrentStockQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetCurrentStockQuery request, CancellationToken cancellationToken)
    {
        var result = await repository.GetCurrentStockAsync(request.ProductId, request.WarehouseId ?? Guid.Empty, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to get current stock for product {ProductId} in warehouse {WarehouseId}: {Error}", request.ProductId, request.WarehouseId, result.ErrorMessage);
            return Result<int>.Failure(result.ErrorMessage ?? "Failed to get current stock");
        }

        var value = result.Data;
        return Result<int>.Success((int)value);
    }
}


