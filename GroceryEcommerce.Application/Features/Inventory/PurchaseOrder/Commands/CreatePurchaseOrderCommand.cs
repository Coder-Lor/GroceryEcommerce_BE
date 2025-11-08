using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Commands;

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    Guid? WarehouseId,
    DateTime? ExpectedDate,
    List<CreatePurchaseOrderItemRequest> Items
) : IRequest<Result<PurchaseOrderDto>>;


