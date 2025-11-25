using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class DeleteOrderHandler(
    ISalesRepository repository,
    ILogger<DeleteOrderHandler> logger
) : IRequestHandler<DeleteOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order: {OrderId}", request.OrderId);

            var result = await repository.DeleteOrderAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order: {OrderId}", request.OrderId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order.");
            }

            logger.LogInformation("Order deleted successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while deleting the order.");
        }
    }
}

