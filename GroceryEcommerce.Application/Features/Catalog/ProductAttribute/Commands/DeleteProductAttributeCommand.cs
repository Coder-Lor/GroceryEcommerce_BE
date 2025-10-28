using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Commands;

public record DeleteProductAttributeCommand(
    Guid AttributeId
) : IRequest<Result<bool>>;
