using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;

public record DeleteShipmentCarrierCommand(
    Guid CarrierId
) : IRequest<Result<bool>>;

