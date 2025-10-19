using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Commands;

public record RemoveAllTagsFromProductCommand(
    Guid ProductId
) : IRequest<Result<bool>>;
