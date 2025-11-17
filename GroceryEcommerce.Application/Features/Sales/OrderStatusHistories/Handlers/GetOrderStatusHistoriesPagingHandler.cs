using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class GetOrderStatusHistoriesPagingHandler(
    IMapper mapper,
    IOrderStatusHistoryRepository repository,
    ILogger<GetOrderStatusHistoriesPagingHandler> logger
) : IRequestHandler<GetOrderStatusHistoriesPagingQuery, Result<PagedResult<OrderStatusHistoryDto>>>
{
    public async Task<Result<PagedResult<OrderStatusHistoryDto>>> Handle(GetOrderStatusHistoriesPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged order status histories - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderStatusHistoryDto>>.Failure(result.ErrorMessage ?? "Failed to get paged order status histories.");
            }

            var mappedItems = mapper.Map<List<OrderStatusHistoryDto>>(result.Data.Items);
            var response = new PagedResult<OrderStatusHistoryDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderStatusHistoryDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged order status histories");
            return Result<PagedResult<OrderStatusHistoryDto>>.Failure("An error occurred while retrieving order status histories.");
        }
    }
}

