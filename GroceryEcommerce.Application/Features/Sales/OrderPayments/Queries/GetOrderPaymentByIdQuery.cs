using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;

public record GetOrderPaymentByIdQuery(
    Guid PaymentId
) : IRequest<Result<OrderPaymentDto>>;

