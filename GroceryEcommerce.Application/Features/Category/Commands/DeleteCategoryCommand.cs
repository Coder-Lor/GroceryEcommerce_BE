using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Category.Commands;

public record DeleteCategoryCommand(
    Guid CategoryId
) : IRequest<Result<bool>>;
