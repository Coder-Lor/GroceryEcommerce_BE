using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class GetOrderStatusHistoriesByOrderIdHandler(
    IMapper mapper,
    IOrderStatusHistoryRepository repository,
    ILogger<GetOrderStatusHistoriesByOrderIdHandler> logger
) : IRequestHandler<GetOrderStatusHistoriesByOrderIdQuery, Result<List<OrderStatusHistoryDto>>>
{
    public async Task<Result<List<OrderStatusHistoryDto>>> Handle(GetOrderStatusHistoriesByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order status histories for order: {OrderId}", request.OrderId);

            var result = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<OrderStatusHistoryDto>>.Failure(result.ErrorMessage ?? "Failed to get order status histories.");
            }

            var response = mapper.Map<List<OrderStatusHistoryDto>>(result.Data);
            return Result<List<OrderStatusHistoryDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order status histories for order: {OrderId}", request.OrderId);
            return Result<List<OrderStatusHistoryDto>>.Failure("An error occurred while retrieving order status histories.");
        }
    }
}

