using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class GetOrderItemByIdHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    ILogger<GetOrderItemByIdHandler> logger
) : IRequestHandler<GetOrderItemByIdQuery, Result<OrderItemDto>>
{
    public async Task<Result<OrderItemDto>> Handle(GetOrderItemByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order item: {OrderItemId}", request.OrderItemId);

            var result = await repository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderItemDto>.Failure("Order item not found.");
            }

            var response = mapper.Map<OrderItemDto>(result.Data);
            return Result<OrderItemDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order item: {OrderItemId}", request.OrderItemId);
            return Result<OrderItemDto>.Failure("An error occurred while retrieving order item.");
        }
    }
}

