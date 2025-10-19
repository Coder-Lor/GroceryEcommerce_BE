using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Queries;

public record GetProductTagAssignmentsByTagQuery(
    Guid TagId,
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection
) : IRequest<Result<PagedResult<ProductTagAssignmentDto>>>;
