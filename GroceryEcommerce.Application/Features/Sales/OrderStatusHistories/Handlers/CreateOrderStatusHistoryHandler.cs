using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Handlers;

public class CreateOrderStatusHistoryHandler(
    IMapper mapper,
    IOrderStatusHistoryRepository repository,
    ILogger<CreateOrderStatusHistoryHandler> logger
) : IRequestHandler<CreateOrderStatusHistoryCommand, Result<OrderStatusHistoryDto>>
{
    public async Task<Result<OrderStatusHistoryDto>> Handle(CreateOrderStatusHistoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order status history for order: {OrderId}", request.Request.OrderId);

            var history = mapper.Map<OrderStatusHistory>(request.Request);
            history.HistoryId = Guid.NewGuid();
            history.CreatedAt = DateTime.UtcNow;

            var createResult = await repository.CreateAsync(history, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order status history");
                return Result<OrderStatusHistoryDto>.Failure(createResult.ErrorMessage ?? "Failed to create order status history.");
            }

            var getResult = await repository.GetByIdAsync(history.HistoryId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<OrderStatusHistoryDto>.Failure("Order status history created but could not be retrieved.");
            }

            var response = mapper.Map<OrderStatusHistoryDto>(getResult.Data);
            logger.LogInformation("Order status history created successfully: {HistoryId}", history.HistoryId);
            return Result<OrderStatusHistoryDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order status history");
            return Result<OrderStatusHistoryDto>.Failure("An error occurred while creating order status history.");
        }
    }
}

