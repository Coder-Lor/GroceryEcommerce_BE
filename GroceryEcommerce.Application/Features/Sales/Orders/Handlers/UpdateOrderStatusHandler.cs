using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class UpdateOrderStatusHandler(
    ISalesRepository repository,
    ILogger<UpdateOrderStatusHandler> logger
) : IRequestHandler<UpdateOrderStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order status: {OrderId}, Status: {Status}", request.OrderId, request.Status);

            var result = await repository.UpdateOrderStatusAsync(request.OrderId, request.Status, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update order status: {OrderId}", request.OrderId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update order status.");
            }

            logger.LogInformation("Order status updated successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order status: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while updating order status.");
        }
    }
}

