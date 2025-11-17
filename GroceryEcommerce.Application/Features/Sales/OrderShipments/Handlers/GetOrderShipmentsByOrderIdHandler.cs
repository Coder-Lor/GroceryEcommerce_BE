using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class GetOrderShipmentsByOrderIdHandler(
    IMapper mapper,
    IOrderShipmentRepository repository,
    ILogger<GetOrderShipmentsByOrderIdHandler> logger
) : IRequestHandler<GetOrderShipmentsByOrderIdQuery, Result<List<OrderShipmentDto>>>
{
    public async Task<Result<List<OrderShipmentDto>>> Handle(GetOrderShipmentsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order shipments for order: {OrderId}", request.OrderId);

            var result = await repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<List<OrderShipmentDto>>.Failure(result.ErrorMessage ?? "Failed to get order shipments.");
            }

            var response = mapper.Map<List<OrderShipmentDto>>(result.Data);
            return Result<List<OrderShipmentDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order shipments for order: {OrderId}", request.OrderId);
            return Result<List<OrderShipmentDto>>.Failure("An error occurred while retrieving order shipments.");
        }
    }
}

