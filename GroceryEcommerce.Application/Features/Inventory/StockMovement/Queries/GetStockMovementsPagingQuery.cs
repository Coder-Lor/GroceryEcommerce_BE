using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;

public record GetStockMovementsPagingQuery(PagedRequest Request) : IRequest<Result<PagedResult<StockMovementDto>>>;


