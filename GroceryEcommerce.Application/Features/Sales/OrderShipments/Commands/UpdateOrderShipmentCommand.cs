using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;

public record UpdateOrderShipmentCommand(
    Guid ShipmentId,
    UpdateOrderShipmentRequest Request
) : IRequest<Result<bool>>;

