using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;

public record DeleteOrderItemCommand(
    Guid OrderItemId
) : IRequest<Result<bool>>;

