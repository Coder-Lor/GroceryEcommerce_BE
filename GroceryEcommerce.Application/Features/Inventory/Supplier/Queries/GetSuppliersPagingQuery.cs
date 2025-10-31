using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Inventory;
using MediatR;

namespace GroceryEcommerce.Application.Features.Inventory.Supplier.Queries;

public record GetSuppliersPagingQuery(PagedRequest Request) : IRequest<Result<PagedResult<SupplierDto>>>;


