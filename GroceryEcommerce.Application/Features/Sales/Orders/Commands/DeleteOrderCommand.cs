using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Commands;

public record DeleteOrderCommand(
    Guid OrderId
) : IRequest<Result<bool>>;

