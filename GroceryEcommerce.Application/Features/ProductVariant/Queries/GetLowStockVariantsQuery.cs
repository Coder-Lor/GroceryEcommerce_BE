using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductVariant.Queries;

public record GetLowStockVariantsQuery(
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection,
    int Threshold = 10
) : IRequest<Result<PagedResult<ProductVariantDto>>>;
