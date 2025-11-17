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

            // Get order item first to validate it exists and log product information
            var orderItemResult = await repository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (!orderItemResult.IsSuccess || orderItemResult.Data is null)
            {
                logger.LogWarning("Order item not found: {OrderItemId}", request.OrderItemId);
                return Result<bool>.Failure("Order item not found.");
            }

            var orderItem = orderItemResult.Data;
            
            // Log product information before deletion for audit purposes
            logger.LogInformation(
                "Deleting order item - OrderId: {OrderId}, ProductId: {ProductId}, ProductName: {ProductName}, ProductSku: {ProductSku}, Quantity: {Quantity}, UnitPrice: {UnitPrice}",
                orderItem.OrderId,
                orderItem.ProductId,
                orderItem.ProductName,
                orderItem.ProductSku,
                orderItem.Quantity,
                orderItem.UnitPrice
            );

            var result = await repository.DeleteAsync(request.OrderItemId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order item: {OrderItemId}", request.OrderItemId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order item.");
            }

            logger.LogInformation(
                "Order item deleted successfully - OrderItemId: {OrderItemId}, ProductName: {ProductName}, Quantity: {Quantity}",
                request.OrderItemId,
                orderItem.ProductName,
                orderItem.Quantity
            );
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order item: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Failure("An error occurred while deleting order item.");
        }
    }
}

