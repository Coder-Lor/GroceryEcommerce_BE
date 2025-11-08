using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Queries;

public record GetActiveWarehousesPagingQuery(PagedRequest Request) : IRequest<Result<PagedResult<WarehouseDto>>>;

