using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Queries;

public record GetProductAttributeValuesByAttributeQuery(
    Guid AttributeId,
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection
) : IRequest<Result<PagedResult<ProductAttributeValueDto>>>;


