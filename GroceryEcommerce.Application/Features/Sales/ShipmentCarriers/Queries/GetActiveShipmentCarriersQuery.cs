using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;

public record GetActiveShipmentCarriersQuery() : IRequest<Result<List<ShipmentCarrierDto>>>;

