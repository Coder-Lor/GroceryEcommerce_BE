using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;

public record CreateWarehouseCommand(
    string Name,
    string? Code,
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? Phone,
    bool IsActive = true
) : IRequest<Result<WarehouseDto>>;

