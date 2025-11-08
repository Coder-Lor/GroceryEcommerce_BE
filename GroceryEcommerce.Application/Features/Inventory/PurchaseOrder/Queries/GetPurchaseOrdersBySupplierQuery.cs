using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;

public record GetPurchaseOrdersBySupplierQuery(Guid SupplierId, PagedRequest Request) : IRequest<Result<PagedResult<PurchaseOrderDto>>>;


