using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record CheckCategoryExistsQuery(
    string Name
) : IRequest<Result<bool>>;
