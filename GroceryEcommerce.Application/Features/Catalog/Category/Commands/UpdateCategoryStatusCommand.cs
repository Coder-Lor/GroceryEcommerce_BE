using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Brand.Queries;

public record UpdateCategoryStatusCommand(
    Guid CategoryId,
    short Status
) : IRequest<Result<bool>>;
