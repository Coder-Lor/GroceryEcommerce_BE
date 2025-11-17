using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Commands;

public record CreateOrderCommand(
    CreateOrderRequest Request
) : IRequest<Result<OrderDto>>;

