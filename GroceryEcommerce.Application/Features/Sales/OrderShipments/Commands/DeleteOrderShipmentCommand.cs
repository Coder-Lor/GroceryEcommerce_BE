using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;

public record DeleteOrderShipmentCommand(
    Guid ShipmentId
) : IRequest<Result<bool>>;

