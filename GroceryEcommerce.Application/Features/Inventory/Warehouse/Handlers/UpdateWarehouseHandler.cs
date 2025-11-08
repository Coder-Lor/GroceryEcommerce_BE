using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class UpdateWarehouseHandler(
    IWarehouseRepository repository,
    IMapper mapper,
    ILogger<UpdateWarehouseHandler> logger
) : IRequestHandler<UpdateWarehouseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating warehouse: {WarehouseId}", request.WarehouseId);

        var existingResult = await repository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            logger.LogWarning("Warehouse not found: {WarehouseId}", request.WarehouseId);
            return Result<bool>.Failure("Warehouse not found");
        }

        var warehouse = existingResult.Data;
        warehouse.Name = request.Name;
        warehouse.Code = request.Code;
        warehouse.Address = request.Address;
        warehouse.City = request.City;
        warehouse.State = request.State;
        warehouse.Country = request.Country;
        warehouse.Phone = request.Phone;
        warehouse.IsActive = request.IsActive;

        var updateResult = await repository.UpdateAsync(warehouse, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update warehouse: {WarehouseId}", request.WarehouseId);
            return Result<bool>.Failure(updateResult.ErrorMessage);
        }

        logger.LogInformation("Warehouse updated successfully: {WarehouseId}", request.WarehouseId);
        return Result<bool>.Success(true);
    }
}


