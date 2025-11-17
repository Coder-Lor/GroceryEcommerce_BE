using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Commands;

public record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    short Status
) : IRequest<Result<bool>>;

