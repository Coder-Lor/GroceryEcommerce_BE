using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class DeleteOrderStatusHistoryHandler(
    IOrderStatusHistoryRepository repository,
    ILogger<DeleteOrderStatusHistoryHandler> logger
) : IRequestHandler<DeleteOrderStatusHistoryCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderStatusHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order status history: {HistoryId}", request.HistoryId);

            var result = await repository.DeleteAsync(request.HistoryId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order status history: {HistoryId}", request.HistoryId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order status history.");
            }

            logger.LogInformation("Order status history deleted successfully: {HistoryId}", request.HistoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order status history: {HistoryId}", request.HistoryId);
            return Result<bool>.Failure("An error occurred while deleting order status history.");
        }
    }
}

