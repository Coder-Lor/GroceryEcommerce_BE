using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Queries;

public record GetProductsByBrandQuery(
    Guid BrandId,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "Name",
    string? SortDirection = "Asc"
) : IRequest<Result<GetProductsByBrandResponse>>;
