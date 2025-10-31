using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;

public record UpdateSupplierCommand(
    Guid SupplierId,
    string Name,
    string? ContactName,
    string? ContactEmail,
    string? ContactPhone,
    string? Address,
    string? Note
) : IRequest<Result<bool>>;


