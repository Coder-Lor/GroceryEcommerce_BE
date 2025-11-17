using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Commands;

public record UpdateOrderStatusCommand(
    Guid OrderId,
    short Status
) : IRequest<Result<bool>>;

