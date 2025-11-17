using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class GetActiveShipmentCarriersHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<GetActiveShipmentCarriersHandler> logger
) : IRequestHandler<GetActiveShipmentCarriersQuery, Result<List<ShipmentCarrierDto>>>
{
    public async Task<Result<List<ShipmentCarrierDto>>> Handle(GetActiveShipmentCarriersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting active shipment carriers");

            var result = await repository.GetActiveCarriersAsync(cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<ShipmentCarrierDto>>.Failure(result.ErrorMessage ?? "Failed to get active shipment carriers.");
            }

            var response = mapper.Map<List<ShipmentCarrierDto>>(result.Data);
            return Result<List<ShipmentCarrierDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active shipment carriers");
            return Result<List<ShipmentCarrierDto>>.Failure("An error occurred while retrieving active shipment carriers.");
        }
    }
}

