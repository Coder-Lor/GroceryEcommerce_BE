using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class UpdateShipmentCarrierHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<UpdateShipmentCarrierHandler> logger
) : IRequestHandler<UpdateShipmentCarrierCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateShipmentCarrierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating shipment carrier: {CarrierId}", request.CarrierId);

            var carrierResult = await repository.GetByIdAsync(request.CarrierId, cancellationToken);
            if (!carrierResult.IsSuccess || carrierResult.Data is null)
            {
                return Result<bool>.Failure("Shipment carrier not found.");
            }

            var carrier = carrierResult.Data;
            carrier.Name = request.Request.Name;
            carrier.Code = request.Request.Code;
            carrier.Website = request.Request.Website;
            carrier.Phone = request.Request.Phone;

            var updateResult = await repository.UpdateAsync(carrier, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update shipment carrier: {CarrierId}", request.CarrierId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update shipment carrier.");
            }

            logger.LogInformation("Shipment carrier updated successfully: {CarrierId}", request.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating shipment carrier: {CarrierId}", request.CarrierId);
            return Result<bool>.Failure("An error occurred while updating shipment carrier.");
        }
    }
}

