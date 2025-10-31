using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;

public record UpdatePurchaseOrderStatusCommand(
    Guid PurchaseOrderId,
    short Status
) : IRequest<Result<bool>>;


