using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;

public record GetProductAttributeByIdQuery(
    Guid AttributeId
) : IRequest<Result<ProductAttributeDto>>;
