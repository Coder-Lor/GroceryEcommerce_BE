using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class GetOrderShipmentByIdHandler(
    IMapper mapper,
    IOrderShipmentRepository repository,
    ILogger<GetOrderShipmentByIdHandler> logger
) : IRequestHandler<GetOrderShipmentByIdQuery, Result<OrderShipmentDto>>
{
    public async Task<Result<OrderShipmentDto>> Handle(GetOrderShipmentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting order shipment: {ShipmentId}", request.ShipmentId);

            var result = await repository.GetByIdAsync(request.ShipmentId, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<OrderShipmentDto>.Failure("Order shipment not found.");
            }

            var response = mapper.Map<OrderShipmentDto>(result.Data);
            return Result<OrderShipmentDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order shipment: {ShipmentId}", request.ShipmentId);
            return Result<OrderShipmentDto>.Failure("An error occurred while retrieving order shipment.");
        }
    }
}

