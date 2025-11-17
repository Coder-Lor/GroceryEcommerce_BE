using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class GetShipmentCarrierByIdHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<GetShipmentCarrierByIdHandler> logger
) : IRequestHandler<GetShipmentCarrierByIdQuery, Result<ShipmentCarrierDto>>
{
    public async Task<Result<ShipmentCarrierDto>> Handle(GetShipmentCarrierByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting shipment carrier: {CarrierId}", request.CarrierId);

            var result = await repository.GetByIdAsync(request.CarrierId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<ShipmentCarrierDto>.Failure("Shipment carrier not found.");
            }

            var response = mapper.Map<ShipmentCarrierDto>(result.Data);
            return Result<ShipmentCarrierDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting shipment carrier: {CarrierId}", request.CarrierId);
            return Result<ShipmentCarrierDto>.Failure("An error occurred while retrieving shipment carrier.");
        }
    }
}

