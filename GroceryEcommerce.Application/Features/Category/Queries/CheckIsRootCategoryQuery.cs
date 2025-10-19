using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Queries;

public record CheckIsRootCategoryQuery(
    Guid CategoryId
) : IRequest<Result<bool>>;
