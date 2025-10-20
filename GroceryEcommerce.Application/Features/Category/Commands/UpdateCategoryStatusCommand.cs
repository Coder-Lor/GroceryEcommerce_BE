using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Commands;

public record UpdateCategoryStatusCommand(
    Guid CategoryId,
    short Status
) : IRequest<Result<bool>>;
