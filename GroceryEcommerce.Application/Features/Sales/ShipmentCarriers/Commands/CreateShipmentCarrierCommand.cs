using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Commands;

public record CreateShipmentCarrierCommand(
    CreateShipmentCarrierRequest Request
) : IRequest<Result<ShipmentCarrierDto>>;

