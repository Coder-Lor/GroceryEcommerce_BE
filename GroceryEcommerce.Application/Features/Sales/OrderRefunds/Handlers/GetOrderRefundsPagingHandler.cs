using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class GetOrderRefundsPagingHandler(
    IMapper mapper,
    IOrderRefundRepository repository,
    ILogger<GetOrderRefundsPagingHandler> logger
) : IRequestHandler<GetOrderRefundsPagingQuery, Result<PagedResult<OrderRefundDto>>>
{
    public async Task<Result<PagedResult<OrderRefundDto>>> Handle(GetOrderRefundsPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged order refunds - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderRefundDto>>.Failure(result.ErrorMessage ?? "Failed to get paged order refunds.");
            }

            var mappedItems = mapper.Map<List<OrderRefundDto>>(result.Data.Items);
            var response = new PagedResult<OrderRefundDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderRefundDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged order refunds");
            return Result<PagedResult<OrderRefundDto>>.Failure("An error occurred while retrieving order refunds.");
        }
    }
}

