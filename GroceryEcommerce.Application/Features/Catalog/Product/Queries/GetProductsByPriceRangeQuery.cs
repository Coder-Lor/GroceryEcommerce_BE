using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Queries;

public record GetProductsByPriceRangeQuery(
    decimal MinPrice,
    decimal MaxPrice,
    string? SortBy = "Price",
    string? SortDirection = "Asc",
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<GetProductsByPriceRangeResponse>>;