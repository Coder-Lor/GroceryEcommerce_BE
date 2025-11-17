using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class GetOrderShipmentsPagingHandler(
    IMapper mapper,
    IOrderShipmentRepository repository,
    ILogger<GetOrderShipmentsPagingHandler> logger
) : IRequestHandler<GetOrderShipmentsPagingQuery, Result<PagedResult<OrderShipmentDto>>>
{
    public async Task<Result<PagedResult<OrderShipmentDto>>> Handle(GetOrderShipmentsPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged order shipments - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderShipmentDto>>.Failure(result.ErrorMessage ?? "Failed to get paged order shipments.");
            }

            var mappedItems = mapper.Map<List<OrderShipmentDto>>(result.Data.Items);
            var response = new PagedResult<OrderShipmentDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderShipmentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged order shipments");
            return Result<PagedResult<OrderShipmentDto>>.Failure("An error occurred while retrieving order shipments.");
        }
    }
}

