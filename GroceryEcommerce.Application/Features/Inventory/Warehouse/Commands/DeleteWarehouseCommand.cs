using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Warehouse.Commands;

public record DeleteWarehouseCommand(Guid WarehouseId) : IRequest<Result<bool>>;

