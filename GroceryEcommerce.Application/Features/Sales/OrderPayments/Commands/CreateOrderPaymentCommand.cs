using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;

public record CreateOrderPaymentCommand(
    CreateOrderPaymentRequest Request
) : IRequest<Result<OrderPaymentDto>>;

