using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderShipments.Queries;

public record GetOrderShipmentsByOrderIdQuery(
    Guid OrderId
) : IRequest<Result<List<OrderShipmentDto>>>;

