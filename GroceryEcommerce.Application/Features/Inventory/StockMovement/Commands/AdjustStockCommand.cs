using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.StockMovement.Commands;

public record AdjustStockCommand(
    Guid ProductId,
    int Quantity,
    string Reason
) : IRequest<Result<bool>>;


