using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;

public record GetCurrentStockQuery(Guid ProductId, Guid? WarehouseId) : IRequest<Result<int>>;


