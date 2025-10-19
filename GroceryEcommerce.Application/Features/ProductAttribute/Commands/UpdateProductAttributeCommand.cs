using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Commands;

public record UpdateProductAttributeCommand(
    Guid AttributeId,
    string Name,
    string DisplayName,
    int AttributeType,
    bool IsRequired,
    int DisplayOrder
) : IRequest<Result<UpdateProductAttributeResponse>>;
