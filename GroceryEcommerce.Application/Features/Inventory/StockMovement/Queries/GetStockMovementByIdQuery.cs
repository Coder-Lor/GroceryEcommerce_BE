using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Queries;

public record GetStockMovementByIdQuery(Guid MovementId) : IRequest<Result<StockMovementDto>>;


