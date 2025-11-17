using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class UpdateShipmentStatusHandler(
    IOrderShipmentRepository repository,
    ILogger<UpdateShipmentStatusHandler> logger
) : IRequestHandler<UpdateShipmentStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating shipment status: {ShipmentId}, Status: {Status}", request.ShipmentId, request.Status);

            var result = await repository.UpdateShipmentStatusAsync(request.ShipmentId, request.Status, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update shipment status: {ShipmentId}", request.ShipmentId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update shipment status.");
            }

            logger.LogInformation("Shipment status updated successfully: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating shipment status: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Failure("An error occurred while updating shipment status.");
        }
    }
}

