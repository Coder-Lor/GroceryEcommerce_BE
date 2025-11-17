using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;

public record UpdatePaymentStatusCommand(
    Guid PaymentId,
    short Status
) : IRequest<Result<bool>>;

