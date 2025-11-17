using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class GetOrderItemsByOrderIdHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    ILogger<GetOrderItemsByOrderIdHandler> logger
) : IRequestHandler<GetOrderItemsByOrderIdQuery, Result<List<OrderItemDto>>>
{
    public async Task<Result<List<OrderItemDto>>> Handle(GetOrderItemsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order items for order: {OrderId}", request.OrderId);

            var result = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<OrderItemDto>>.Failure(result.ErrorMessage ?? "Failed to get order items.");
            }

            var response = mapper.Map<List<OrderItemDto>>(result.Data);
            return Result<List<OrderItemDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order items for order: {OrderId}", request.OrderId);
            return Result<List<OrderItemDto>>.Failure("An error occurred while retrieving order items.");
        }
    }
}

