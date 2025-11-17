using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class DeleteOrderShipmentHandler(
    IOrderShipmentRepository repository,
    ILogger<DeleteOrderShipmentHandler> logger
) : IRequestHandler<DeleteOrderShipmentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderShipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order shipment: {ShipmentId}", request.ShipmentId);

            var result = await repository.DeleteAsync(request.ShipmentId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order shipment: {ShipmentId}", request.ShipmentId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order shipment.");
            }

            logger.LogInformation("Order shipment deleted successfully: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order shipment: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Failure("An error occurred while deleting order shipment.");
        }
    }
}

