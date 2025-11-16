using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;

public record GetShipmentCarrierByIdQuery(
    Guid CarrierId
) : IRequest<Result<ShipmentCarrierDto>>;

