using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class DeleteOrderItemHandler(
    IOrderItemRepository repository,
    ILogger<DeleteOrderItemHandler> logger
) : IRequestHandler<DeleteOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order item: {OrderItemId}", request.OrderItemId);

            var result = await repository.DeleteAsync(request.OrderItemId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order item: {OrderItemId}", request.OrderItemId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order item.");
            }

            logger.LogInformation("Order item deleted successfully: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order item: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Failure("An error occurred while deleting order item.");
        }
    }
}

