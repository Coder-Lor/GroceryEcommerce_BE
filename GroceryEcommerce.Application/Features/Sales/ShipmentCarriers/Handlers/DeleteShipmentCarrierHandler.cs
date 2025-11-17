using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class DeleteShipmentCarrierHandler(
    IShipmentCarrierRepository repository,
    ILogger<DeleteShipmentCarrierHandler> logger
) : IRequestHandler<DeleteShipmentCarrierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteShipmentCarrierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting shipment carrier: {CarrierId}", request.CarrierId);

            // Check if carrier is in use
            var inUseResult = await repository.IsCarrierInUseAsync(request.CarrierId, cancellationToken);
            if (inUseResult.IsSuccess && inUseResult.Data)
            {
                return Result<bool>.Failure("Cannot delete carrier that is in use.");
            }

            var result = await repository.DeleteAsync(request.CarrierId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete shipment carrier: {CarrierId}", request.CarrierId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete shipment carrier.");
            }

            logger.LogInformation("Shipment carrier deleted successfully: {CarrierId}", request.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting shipment carrier: {CarrierId}", request.CarrierId);
            return Result<bool>.Failure("An error occurred while deleting shipment carrier.");
        }
    }
}

