using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class UpdateOrderItemHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    ILogger<UpdateOrderItemHandler> logger
) : IRequestHandler<UpdateOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order item: {OrderItemId}", request.OrderItemId);

            var orderItemResult = await repository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (!orderItemResult.IsSuccess || orderItemResult.Data is null)
            {
                return Result<bool>.Failure("Order item not found.");
            }

            var orderItem = orderItemResult.Data;
            orderItem.UnitPrice = request.Request.UnitPrice;
            orderItem.Quantity = request.Request.Quantity;
            orderItem.TotalPrice = request.Request.UnitPrice * request.Request.Quantity;

            var updateResult = await repository.UpdateAsync(orderItem, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order item: {OrderItemId}", request.OrderItemId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order item.");
            }

            logger.LogInformation("Order item updated successfully: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order item: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Failure("An error occurred while updating order item.");
        }
    }
}

