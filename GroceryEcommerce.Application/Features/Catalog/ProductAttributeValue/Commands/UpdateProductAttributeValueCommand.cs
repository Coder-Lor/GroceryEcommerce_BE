using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Commands;

public record UpdateProductAttributeValueCommand(
    Guid ValueId,
    Guid ProductId,
    Guid AttributeId,
    string Value
) : IRequest<Result<UpdateProductAttributeValueResponse>>;
