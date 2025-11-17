using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Queries;

public record GetOrderStatusHistoriesPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderStatusHistoryDto>>>;

