using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Commands;

public record DeleteSupplierCommand(Guid SupplierId) : IRequest<Result<bool>>;


