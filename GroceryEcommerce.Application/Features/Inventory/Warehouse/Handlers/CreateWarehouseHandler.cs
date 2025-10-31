using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Handlers;

public class CreateWarehouseHandler(
    IWarehouseRepository repository,
    IMapper mapper,
    ILogger<CreateWarehouseHandler> logger
) : IRequestHandler<CreateWarehouseCommand, Result<WarehouseDto>>
{
    public async Task<Result<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating warehouse: {Name}", request.Name);

        if (!string.IsNullOrEmpty(request.Code))
        {
            var existingResult = await repository.GetByCodeAsync(request.Code, cancellationToken);
            if (existingResult.IsSuccess && existingResult.Data != null)
            {
                logger.LogWarning("Warehouse with code {Code} already exists", request.Code);
                return Result<WarehouseDto>.Failure("Warehouse with this code already exists");
            }
        }

        var warehouse = new Domain.Entities.Inventory.Warehouse
        {
            WarehouseId = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Phone = request.Phone,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await repository.CreateAsync(warehouse, cancellationToken);
        if (!createResult.IsSuccess)
        {
            logger.LogError("Failed to create warehouse: {Name}", request.Name);
            return Result<WarehouseDto>.Failure(createResult.ErrorMessage);
        }

        var dto = mapper.Map<WarehouseDto>(createResult.Data);
        logger.LogInformation("Warehouse created successfully: {WarehouseId}", dto.WarehouseId);
        return Result<WarehouseDto>.Success(dto);
    }
}


