using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;

public record SearchSuppliersByNameQuery(string SearchTerm) : IRequest<Result<List<SupplierDto>>>;


