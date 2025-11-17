using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class GetAllShipmentCarriersHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<GetAllShipmentCarriersHandler> logger
) : IRequestHandler<GetAllShipmentCarriersQuery, Result<List<ShipmentCarrierDto>>>
{
    public async Task<Result<List<ShipmentCarrierDto>>> Handle(GetAllShipmentCarriersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting all shipment carriers");

            var result = await repository.GetAllAsync(cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<ShipmentCarrierDto>>.Failure(result.ErrorMessage ?? "Failed to get shipment carriers.");
            }

            var response = mapper.Map<List<ShipmentCarrierDto>>(result.Data);
            return Result<List<ShipmentCarrierDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all shipment carriers");
            return Result<List<ShipmentCarrierDto>>.Failure("An error occurred while retrieving shipment carriers.");
        }
    }
}

