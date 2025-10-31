using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;

public record GetPurchaseOrderByIdQuery(Guid PurchaseOrderId) : IRequest<Result<PurchaseOrderDto>>;


