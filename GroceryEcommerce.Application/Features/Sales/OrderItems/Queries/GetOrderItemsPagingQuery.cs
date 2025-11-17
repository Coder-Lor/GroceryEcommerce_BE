using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Queries;

public record GetOrderItemsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderItemDto>>>;

