using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;

public record ProcessRefundCommand(
    Guid RefundId,
    Guid ProcessedBy
) : IRequest<Result<bool>>;

