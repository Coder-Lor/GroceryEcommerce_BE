using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Queries;

public record GetProductAttributeValueByIdQuery(
    Guid ValueId
) : IRequest<Result<ProductAttributeValueDto>>;


