using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class DeleteWarehouseHandler(
    IWarehouseRepository repository,
    ILogger<DeleteWarehouseHandler> logger
) : IRequestHandler<DeleteWarehouseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting warehouse: {WarehouseId}", request.WarehouseId);

        var existsResult = await repository.ExistsAsync(request.WarehouseId, cancellationToken);
        if (!existsResult.IsSuccess || !existsResult.Data)
        {
            logger.LogWarning("Warehouse not found: {WarehouseId}", request.WarehouseId);
            return Result<bool>.Failure("Warehouse not found");
        }

        var inUseResult = await repository.IsWarehouseInUseAsync(request.WarehouseId, cancellationToken);
        if (inUseResult.IsSuccess && inUseResult.Data)
        {
            logger.LogWarning("Cannot delete warehouse {WarehouseId} - it is in use", request.WarehouseId);
            return Result<bool>.Failure("Cannot delete warehouse that is in use");
        }

        var deleteResult = await repository.DeleteAsync(request.WarehouseId, cancellationToken);
        if (!deleteResult.IsSuccess)
        {
            logger.LogError("Failed to delete warehouse: {WarehouseId}", request.WarehouseId);
            return Result<bool>.Failure(deleteResult.ErrorMessage);
        }

        logger.LogInformation("Warehouse deleted successfully: {WarehouseId}", request.WarehouseId);
        return Result<bool>.Success(true);
    }
}


