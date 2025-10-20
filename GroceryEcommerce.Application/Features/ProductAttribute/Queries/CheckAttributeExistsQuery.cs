using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Queries;

public record CheckAttributeExistsQuery(
    string Name
) : IRequest<Result<bool>>;
