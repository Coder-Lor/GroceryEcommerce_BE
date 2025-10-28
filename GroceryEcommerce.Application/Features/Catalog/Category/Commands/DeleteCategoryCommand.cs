using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Queries;

public record DeleteCategoryCommand(
    Guid CategoryId
) : IRequest<Result<bool>>;
