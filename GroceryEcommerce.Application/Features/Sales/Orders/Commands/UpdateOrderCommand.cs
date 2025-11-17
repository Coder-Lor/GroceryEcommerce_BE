using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Sales;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Commands;

public record UpdateOrderCommand(
    Guid OrderId,
    UpdateOrderRequest Request
) : IRequest<Result<bool>>;

