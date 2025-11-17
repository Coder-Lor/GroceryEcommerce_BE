using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Handlers;

public class GetShipmentCarriersPagingHandler(
    IMapper mapper,
    IShipmentCarrierRepository repository,
    ILogger<GetShipmentCarriersPagingHandler> logger
) : IRequestHandler<GetShipmentCarriersPagingQuery, Result<PagedResult<ShipmentCarrierDto>>>
{
    public async Task<Result<PagedResult<ShipmentCarrierDto>>> Handle(GetShipmentCarriersPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged shipment carriers - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<ShipmentCarrierDto>>.Failure(result.ErrorMessage ?? "Failed to get paged shipment carriers.");
            }

            var mappedItems = mapper.Map<List<ShipmentCarrierDto>>(result.Data.Items);
            var response = new PagedResult<ShipmentCarrierDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<ShipmentCarrierDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged shipment carriers");
            return Result<PagedResult<ShipmentCarrierDto>>.Failure("An error occurred while retrieving shipment carriers.");
        }
    }
}

