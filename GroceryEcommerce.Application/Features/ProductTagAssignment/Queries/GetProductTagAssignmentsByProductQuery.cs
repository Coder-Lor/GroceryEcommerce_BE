using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Queries;

public record GetProductTagAssignmentsByProductQuery(
    Guid ProductId,
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection
) : IRequest<Result<PagedResult<ProductTagAssignmentDto>>>;
