using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;

public record GetOrderRefundsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderRefundDto>>>;

