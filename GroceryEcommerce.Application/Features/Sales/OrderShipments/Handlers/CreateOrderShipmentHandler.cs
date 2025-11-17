using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Handlers;

public class CreateOrderShipmentHandler(
    IMapper mapper,
    IOrderShipmentRepository repository,
    ILogger<CreateOrderShipmentHandler> logger
) : IRequestHandler<CreateOrderShipmentCommand, Result<OrderShipmentDto>>
{
    public async Task<Result<OrderShipmentDto>> Handle(CreateOrderShipmentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order shipment for order: {OrderId}", request.Request.OrderId);

            // Generate shipment number
            var shipmentNumberResult = await repository.GenerateShipmentNumberAsync(cancellationToken);
            if (!shipmentNumberResult.IsSuccess)
            {
                return Result<OrderShipmentDto>.Failure("Failed to generate shipment number.");
            }

            var orderShipment = mapper.Map<OrderShipment>(request.Request);
            orderShipment.ShipmentId = Guid.NewGuid();
            orderShipment.ShipmentNumber = shipmentNumberResult.Data;
            orderShipment.Status = 1; // Ready
            orderShipment.CreatedAt = DateTime.UtcNow;

            var createResult = await repository.CreateAsync(orderShipment, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order shipment");
                return Result<OrderShipmentDto>.Failure(createResult.ErrorMessage ?? "Failed to create order shipment.");
            }

            var getResult = await repository.GetByIdAsync(orderShipment.ShipmentId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<OrderShipmentDto>.Failure("Order shipment created but could not be retrieved.");
            }

            var response = mapper.Map<OrderShipmentDto>(getResult.Data);
            logger.LogInformation("Order shipment created successfully: {ShipmentId}", orderShipment.ShipmentId);
            return Result<OrderShipmentDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order shipment");
            return Result<OrderShipmentDto>.Failure("An error occurred while creating order shipment.");
        }
    }
}

