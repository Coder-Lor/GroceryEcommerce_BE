using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class GetOrderPaymentsPagingHandler(
    IMapper mapper,
    IOrderPaymentRepository repository,
    ILogger<GetOrderPaymentsPagingHandler> logger
) : IRequestHandler<GetOrderPaymentsPagingQuery, Result<PagedResult<OrderPaymentDto>>>
{
    public async Task<Result<PagedResult<OrderPaymentDto>>> Handle(GetOrderPaymentsPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting paged order payments - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderPaymentDto>>.Failure(result.ErrorMessage ?? "Failed to get paged order payments.");
            }

            var mappedItems = mapper.Map<List<OrderPaymentDto>>(result.Data.Items);
            var response = new PagedResult<OrderPaymentDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderPaymentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged order payments");
            return Result<PagedResult<OrderPaymentDto>>.Failure("An error occurred while retrieving order payments.");
        }
    }
}

