using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;

public record CreateSupplierCommand(
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? Address,
    string? Note
) : IRequest<Result<SupplierDto>>;


