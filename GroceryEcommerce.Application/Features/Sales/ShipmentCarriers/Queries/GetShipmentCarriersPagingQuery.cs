using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.ShipmentCarriers.Queries;

public record GetShipmentCarriersPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<ShipmentCarrierDto>>>;

