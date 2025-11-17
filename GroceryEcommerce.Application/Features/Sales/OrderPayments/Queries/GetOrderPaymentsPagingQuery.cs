using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;

public record GetOrderPaymentsPagingQuery(
    PagedRequest Request
) : IRequest<Result<PagedResult<OrderPaymentDto>>>;

