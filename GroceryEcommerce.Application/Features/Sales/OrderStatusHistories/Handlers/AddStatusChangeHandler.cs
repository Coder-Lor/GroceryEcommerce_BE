using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class AddStatusChangeHandler(
    IOrderStatusHistoryRepository repository,
    ILogger<AddStatusChangeHandler> logger
) : IRequestHandler<AddStatusChangeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddStatusChangeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Adding status change for order: {OrderId}, From: {FromStatus}, To: {ToStatus}", 
                request.OrderId, request.FromStatus, request.ToStatus);

            var result = await repository.AddStatusChangeAsync(
                request.OrderId,
                request.FromStatus,
                request.ToStatus,
                request.Comment,
                request.CreatedBy,
                cancellationToken);

            if (!result.IsSuccess)
            {
                logger.LogError("Failed to add status change for order: {OrderId}", request.OrderId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to add status change.");
            }

            logger.LogInformation("Status change added successfully for order: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding status change for order: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while adding status change.");
        }
    }
}

