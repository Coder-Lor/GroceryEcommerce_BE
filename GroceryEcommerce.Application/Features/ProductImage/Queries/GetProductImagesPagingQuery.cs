using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductImage.Queries;

public record GetProductImagesPagingQuery(
    Guid ProductId,
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection
) : IRequest<Result<PagedResult<ProductImageDto>>>;


