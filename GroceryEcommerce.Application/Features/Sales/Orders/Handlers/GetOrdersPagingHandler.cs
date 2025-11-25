using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class GetOrdersPagingHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrdersPagingHandler> logger
) : IRequestHandler<GetOrdersPagingQuery, Result<PagedResult<OrderDto>>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrdersPagingQuery - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderDto>>.Failure(result.ErrorMessage ?? "Failed to get paged orders.");
            }

            var mappedItems = mapper.Map<List<OrderDto>>(result.Data.Items);
            var response = new PagedResult<OrderDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged orders");
            return Result<PagedResult<OrderDto>>.Failure("An error occurred while retrieving orders.");
        }
    }
}

