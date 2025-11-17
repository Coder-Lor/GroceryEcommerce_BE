using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Sales.OrderStatusHistories.Commands;

public record AddStatusChangeCommand(
    Guid OrderId,
    short FromStatus,
    short ToStatus,
    string? Comment,
    Guid CreatedBy
) : IRequest<Result<bool>>;

