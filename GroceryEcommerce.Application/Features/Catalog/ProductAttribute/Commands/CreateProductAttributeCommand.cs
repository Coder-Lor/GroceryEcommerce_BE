using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Commands;

public record CreateProductAttributeCommand(
    string Name,
    string DisplayName,
    int AttributeType,
    bool IsRequired = false,
    int DisplayOrder = 0
) : IRequest<Result<CreateProductAttributeResponse>>;
