using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class GetOrderStatusHistoryByIdHandler(
    IMapper mapper,
    IOrderStatusHistoryRepository repository,
    ILogger<GetOrderStatusHistoryByIdHandler> logger
) : IRequestHandler<GetOrderStatusHistoryByIdQuery, Result<OrderStatusHistoryDto>>
{
    public async Task<Result<OrderStatusHistoryDto>> Handle(GetOrderStatusHistoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order status history: {HistoryId}", request.HistoryId);

            var result = await repository.GetByIdAsync(request.HistoryId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderStatusHistoryDto>.Failure("Order status history not found.");
            }

            var response = mapper.Map<OrderStatusHistoryDto>(result.Data);
            return Result<OrderStatusHistoryDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order status history: {HistoryId}", request.HistoryId);
            return Result<OrderStatusHistoryDto>.Failure("An error occurred while retrieving order status history.");
        }
    }
}

