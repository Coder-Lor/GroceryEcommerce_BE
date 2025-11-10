using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Commands;

public record CreateStockMovementCommand(
    Guid ProductId,
    short MovementType,
    int Quantity,
    string Reason
) : IRequest<Result<StockMovementDto>>;


