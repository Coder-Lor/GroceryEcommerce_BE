using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Queries;

public record GetProductAttributeByNameQuery(
    string Name
) : IRequest<Result<ProductAttributeDto>>;
