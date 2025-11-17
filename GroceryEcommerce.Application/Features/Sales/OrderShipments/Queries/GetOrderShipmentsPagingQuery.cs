using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;

public record GetOrderShipmentsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderShipmentDto>>>;

