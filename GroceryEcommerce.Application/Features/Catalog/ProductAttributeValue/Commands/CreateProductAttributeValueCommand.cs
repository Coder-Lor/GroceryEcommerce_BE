using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Commands;

public record CreateProductAttributeValueCommand(
    Guid ProductId,
    Guid AttributeId,
    string Value
) : IRequest<Result<CreateProductAttributeValueResponse>>;
