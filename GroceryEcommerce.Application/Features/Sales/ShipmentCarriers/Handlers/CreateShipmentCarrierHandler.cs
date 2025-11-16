using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class CreateShipmentCarrierHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<CreateShipmentCarrierHandler> logger
) : IRequestHandler<CreateShipmentCarrierCommand, Result<ShipmentCarrierDto>>
{
    public async Task<Result<ShipmentCarrierDto>> Handle(CreateShipmentCarrierCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating shipment carrier: {Name}", request.Request.Name);

            var carrier = mapper.Map<ShipmentCarrier>(request.Request);
            carrier.CarrierId = Guid.NewGuid();
            carrier.CreatedAt = DateTime.UtcNow;

            var createResult = await repository.CreateAsync(carrier, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create shipment carrier");
                return Result<ShipmentCarrierDto>.Failure(createResult.ErrorMessage ?? "Failed to create shipment carrier.");
            }

            var getResult = await repository.GetByIdAsync(carrier.CarrierId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<ShipmentCarrierDto>.Failure("Shipment carrier created but could not be retrieved.");
            }

            var response = mapper.Map<ShipmentCarrierDto>(getResult.Data);
            logger.LogInformation("Shipment carrier created successfully: {CarrierId}", carrier.CarrierId);
            return Result<ShipmentCarrierDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating shipment carrier");
            return Result<ShipmentCarrierDto>.Failure("An error occurred while creating shipment carrier.");
        }
    }
}

