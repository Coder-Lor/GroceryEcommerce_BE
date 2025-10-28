using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Category.Queries;

public record GetProductCountByCategoryQuery(
    Guid CategoryId
) : IRequest<Result<int>>;
