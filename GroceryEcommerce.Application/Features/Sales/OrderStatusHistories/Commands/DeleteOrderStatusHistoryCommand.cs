using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;

public record DeleteOrderStatusHistoryCommand(
    Guid HistoryId
) : IRequest<Result<bool>>;

