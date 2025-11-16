using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;

public record DeleteOrderRefundCommand(
    Guid RefundId
) : IRequest<Result<bool>>;

