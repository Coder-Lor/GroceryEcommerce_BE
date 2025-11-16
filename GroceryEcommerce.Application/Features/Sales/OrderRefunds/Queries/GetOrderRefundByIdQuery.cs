using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Queries;

public record GetOrderRefundByIdQuery(
    Guid RefundId
) : IRequest<Result<OrderRefundDto>>;

