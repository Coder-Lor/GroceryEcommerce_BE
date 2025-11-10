using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Inventory.StockMovement.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Handlers;

public class AdjustStockHandler(
    IStockMovementRepository repository,
    ICurrentUserService currentUserService,
    IMapper mapper,
    ILogger<AdjustStockHandler> logger
) : IRequestHandler<AdjustStockCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetCurrentUserId();
        if (currentUserId == null)
        {
            return Result<bool>.Failure("Unable to identify current user");
        }

        const short adjustmentMovementType = 3;

        var currentStockResult = await repository.GetCurrentStockAsync(request.ProductId, cancellationToken);
        var currentStock = currentStockResult.IsSuccess ? (int)currentStockResult.Data : 0;
        var newStock = currentStock + request.Quantity;

        var movement = new Domain.Entities.Inventory.StockMovement
        {
            MovementId = Guid.NewGuid(),
            ProductId = request.ProductId,
            MovementType = adjustmentMovementType,
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
            return Result<bool>.Failure(createResult.ErrorMessage);
        }

        return Result<bool>.Success(true);
    }
}


