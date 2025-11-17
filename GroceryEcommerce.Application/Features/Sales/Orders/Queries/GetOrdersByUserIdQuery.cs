using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Queries;

public record GetOrdersByUserIdQuery(
    Guid UserId,
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderDto>>>;

