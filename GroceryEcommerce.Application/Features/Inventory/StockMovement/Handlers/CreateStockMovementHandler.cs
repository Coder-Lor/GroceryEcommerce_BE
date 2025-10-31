using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class CreateStockMovementHandler(
    IStockMovementRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<CreateStockMovementHandler> logger
) : IRequestHandler<CreateStockMovementCommand, Result<StockMovementDto>>
{
    public async Task<Result<StockMovementDto>> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating stock movement for product: {ProductId}", request.ProductId);

        var currentUserId = currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result<StockMovementDto>.Failure("Unable to identify current user");
        }

        var currentStockResult = await repository.GetCurrentStockAsync(request.ProductId, 
            request.WarehouseId ?? Guid.Empty, cancellationToken);
        
        var currentStock = currentStockResult.IsSuccess ? (int)currentStockResult.Data : 0;
        var newStock = currentStock + request.Quantity;

        var movement = new Domain.Entities.Inventory.StockMovement
        {
            MovementId = Guid.NewGuid(),
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            MovementType = request.MovementType,
            Quantity = Math.Abs(request.Quantity),
            PreviousStock = currentStock,
            NewStock = newStock,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = currentUserId.Value
        };

        var createResult = await repository.CreateAsync(movement, cancellationToken);
        if (!createResult.IsSuccess)
        {
            return Result<StockMovementDto>.Failure(createResult.ErrorMessage);
        }

        var dto = mapper.Map<StockMovementDto>(createResult.Data);
        return Result<StockMovementDto>.Success(dto);
    }
}


