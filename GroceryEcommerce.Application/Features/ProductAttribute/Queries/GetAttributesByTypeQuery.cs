using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Queries;

public record GetAttributesByTypeQuery(
    short AttributeType,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "DisplayOrder",
    string? SortDirection = "Asc"
) : IRequest<Result<PagedResult<ProductAttributeDto>>>;
