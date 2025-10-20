using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Commands;

public record DeleteProductAttributeValueCommand(
    Guid ValueId
) : IRequest<Result<bool>>;
