using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Queries;

public record GetProductTagsPagingQuery(
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection,
    string? SearchTerm
) : IRequest<Result<PagedResult<ProductTagDto>>>;
