using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;

public record CheckAttributeExistsByIdQuery(
    Guid AttributeId
) : IRequest<Result<bool>>;
