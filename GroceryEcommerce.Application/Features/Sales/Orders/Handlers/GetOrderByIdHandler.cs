using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class GetOrderByIdHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrderByIdHandler> logger
) : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrderByIdQuery for order: {OrderId}", request.OrderId);

            var orderResult = await repository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (!orderResult.IsSuccess || orderResult.Data is null)
            {
                logger.LogWarning("Order not found: {OrderId}", request.OrderId);
                return Result<OrderDetailDto>.Failure("Order not found");
            }

            var response = mapper.Map<OrderDetailDto>(orderResult.Data);
            logger.LogInformation("Order retrieved successfully: {OrderId}", request.OrderId);
            return Result<OrderDetailDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order by ID: {OrderId}", request.OrderId);
            return Result<OrderDetailDto>.Failure("An error occurred while retrieving the order.");
        }
    }
}

