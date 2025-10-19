using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Queries;

public record SearchProductsQuery(
    string? SearchTerm = null,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsFeatured = null,
    bool? IsActive = null,
    List<Guid>? TagIds = null,
    string? SortBy = "Name",
    string? SortDirection = "Asc",
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<SearchProductsResponse>>;