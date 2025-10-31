using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.PurchaseOrder.Queries;

public record GetPurchaseOrdersByStatusQuery(short Status, PagedRequest Request) : IRequest<Result<PagedResult<PurchaseOrderDto>>>;


