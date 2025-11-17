using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class GetLatestOrderStatusHistoryByOrderIdHandler(
    IMapper mapper,
    IOrderStatusHistoryRepository repository,
    ILogger<GetLatestOrderStatusHistoryByOrderIdHandler> logger
) : IRequestHandler<GetLatestOrderStatusHistoryByOrderIdQuery, Result<OrderStatusHistoryDto>>
{
    public async Task<Result<OrderStatusHistoryDto>> Handle(GetLatestOrderStatusHistoryByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting latest order status history for order: {OrderId}", request.OrderId);

            var result = await repository.GetLatestByOrderAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderStatusHistoryDto>.Failure("Order status history not found.");
            }

            var response = mapper.Map<OrderStatusHistoryDto>(result.Data);
            return Result<OrderStatusHistoryDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting latest order status history for order: {OrderId}", request.OrderId);
            return Result<OrderStatusHistoryDto>.Failure("An error occurred while retrieving order status history.");
        }
    }
}

