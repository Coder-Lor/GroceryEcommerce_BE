using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class UpdateOrderHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<UpdateOrderHandler> logger
) : IRequestHandler<UpdateOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order: {OrderId}", request.OrderId);

            var orderResult = await repository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (!orderResult.IsSuccess || orderResult.Data is null)
            {
                return Result<bool>.Failure("Order not found.");
            }

            var order = orderResult.Data;
            order.Status = request.Request.Status;
            order.PaymentStatus = request.Request.PaymentStatus;
            order.Notes = request.Request.Notes;
            order.EstimatedDeliveryDate = request.Request.EstimatedDeliveryDate;
            order.UpdatedAt = DateTime.UtcNow;

            var updateResult = await repository.UpdateOrderAsync(order, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order: {OrderId}", request.OrderId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order.");
            }

            logger.LogInformation("Order updated successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while updating the order.");
        }
    }
}

