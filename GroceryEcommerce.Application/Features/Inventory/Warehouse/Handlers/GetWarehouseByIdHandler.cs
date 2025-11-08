using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class GetWarehouseByIdHandler(
    IWarehouseRepository repository,
    IMapper mapper,
    ILogger<GetWarehouseByIdHandler> logger
) : IRequestHandler<GetWarehouseByIdQuery, Result<WarehouseDto>>
{
    public async Task<Result<WarehouseDto>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting warehouse by id: {WarehouseId}", request.WarehouseId);

        var result = await repository.GetByIdAsync(request.WarehouseId, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            logger.LogWarning("Warehouse not found: {WarehouseId}", request.WarehouseId);
            return Result<WarehouseDto>.Failure("Warehouse not found");
        }

        var dto = mapper.Map<WarehouseDto>(result.Data);
        return Result<WarehouseDto>.Success(dto);
    }
}


