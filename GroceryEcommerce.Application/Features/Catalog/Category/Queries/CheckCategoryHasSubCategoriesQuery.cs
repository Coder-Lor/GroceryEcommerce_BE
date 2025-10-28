using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Queries;

public record CheckCategoryHasSubCategoriesQuery(
    Guid CategoryId
) : IRequest<Result<bool>>;
