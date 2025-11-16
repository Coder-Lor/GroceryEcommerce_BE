using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class GetShipmentCarrierByNameHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<GetShipmentCarrierByNameHandler> logger
) : IRequestHandler<GetShipmentCarrierByNameQuery, Result<ShipmentCarrierDto>>
{
    public async Task<Result<ShipmentCarrierDto>> Handle(GetShipmentCarrierByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting shipment carrier by name: {Name}", request.Name);

            var result = await repository.GetByNameAsync(request.Name, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<ShipmentCarrierDto>.Failure("Shipment carrier not found.");
            }

            var response = mapper.Map<ShipmentCarrierDto>(result.Data);
            return Result<ShipmentCarrierDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting shipment carrier by name: {Name}", request.Name);
            return Result<ShipmentCarrierDto>.Failure("An error occurred while retrieving shipment carrier.");
        }
    }
}

