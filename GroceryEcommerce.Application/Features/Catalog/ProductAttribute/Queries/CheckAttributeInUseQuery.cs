using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;

public record CheckAttributeInUseQuery(
    Guid AttributeId
) : IRequest<Result<bool>>;
