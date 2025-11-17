using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class UpdateOrderShipmentHandler(
    IMapper mapper,
    IOrderShipmentRepository repository,
    ILogger<UpdateOrderShipmentHandler> logger
) : IRequestHandler<UpdateOrderShipmentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderShipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order shipment: {ShipmentId}", request.ShipmentId);

            var shipmentResult = await repository.GetByIdAsync(request.ShipmentId, cancellationToken);
            if (!shipmentResult.IsSuccess || shipmentResult.Data is null)
            {
                return Result<bool>.Failure("Order shipment not found.");
            }

            var shipment = shipmentResult.Data;
            shipment.CarrierId = request.Request.ShipmentCarrierId;
            shipment.TrackingNumber = request.Request.TrackingNumber;
            shipment.Status = request.Request.Status;
            shipment.UpdatedAt = DateTime.UtcNow;

            var updateResult = await repository.UpdateAsync(shipment, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order shipment: {ShipmentId}", request.ShipmentId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order shipment.");
            }

            logger.LogInformation("Order shipment updated successfully: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order shipment: {ShipmentId}", request.ShipmentId);
            return Result<bool>.Failure("An error occurred while updating order shipment.");
        }
    }
}

