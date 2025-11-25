using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class GetOrdersByUserIdHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrdersByUserIdHandler> logger
) : IRequestHandler<GetOrdersByUserIdQuery, Result<PagedResult<OrderDto>>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrdersByUserIdQuery for user: {UserId}", request.UserId);

            var result = await repository.GetOrdersByUserIdAsync(request.UserId, request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderDto>>.Failure(result.ErrorMessage ?? "Failed to get orders.");
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
            logger.LogError(ex, "Error getting orders by user ID: {UserId}", request.UserId);
            return Result<PagedResult<OrderDto>>.Failure("An error occurred while retrieving orders.");
        }
    }
}

