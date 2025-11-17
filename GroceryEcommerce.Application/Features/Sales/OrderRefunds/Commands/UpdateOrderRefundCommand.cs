using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;

public record UpdateOrderRefundCommand(
    Guid RefundId,
    UpdateOrderRefundRequest Request
) : IRequest<Result<bool>>;

