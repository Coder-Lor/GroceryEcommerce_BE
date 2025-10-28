using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Queries;

public record CheckCategoryExistsByNameQuery(
    string Name
) : IRequest<Result<bool>>;
