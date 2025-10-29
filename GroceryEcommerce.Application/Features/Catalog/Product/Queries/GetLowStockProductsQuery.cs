using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Queries;

public record GetLowStockProductsQuery(
    int Threshold = 10,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "StockQuantity",
    string? SortDirection = "Asc"
) : IRequest<Result<GetLowStockProductsResponse>>;