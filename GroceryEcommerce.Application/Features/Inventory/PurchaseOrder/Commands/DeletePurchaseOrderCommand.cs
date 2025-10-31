using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;

public record DeletePurchaseOrderCommand(Guid PurchaseOrderId) : IRequest<Result<bool>>;


