using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;

public record DeleteOrderPaymentCommand(
    Guid PaymentId
) : IRequest<Result<bool>>;

