using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Queries;

public record CheckCategoryInUseQuery(
    Guid CategoryId
) : IRequest<Result<bool>>;
