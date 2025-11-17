using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class GetOrderItemsPagingHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    ILogger<GetOrderItemsPagingHandler> logger
) : IRequestHandler<GetOrderItemsPagingQuery, Result<PagedResult<OrderItemDto>>>
{
    public async Task<Result<PagedResult<OrderItemDto>>> Handle(GetOrderItemsPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged order items - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderItemDto>>.Failure(result.ErrorMessage ?? "Failed to get paged order items.");
            }

            var mappedItems = mapper.Map<List<OrderItemDto>>(result.Data.Items);
            var response = new PagedResult<OrderItemDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderItemDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged order items");
            return Result<PagedResult<OrderItemDto>>.Failure("An error occurred while retrieving order items.");
        }
    }
}

