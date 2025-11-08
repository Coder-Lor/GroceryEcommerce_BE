using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;

public record UpdateWarehouseCommand(
    Guid WarehouseId,
    string Name,
    string? Code,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? Phone,
    bool IsActive
) : IRequest<Result<bool>>;

