using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttribute.Queries;

public record CheckAttributeExistsQuery(
    string Name
) : IRequest<Result<bool>>;
