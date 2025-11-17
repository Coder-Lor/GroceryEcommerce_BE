using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Queries;

public record GetOrderPaymentsByOrderIdQuery(
    Guid OrderId
) : IRequest<Result<List<OrderPaymentDto>>>;

