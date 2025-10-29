using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTagAssignment.Commands;

public record RemoveAllTagsFromProductCommand(
    Guid ProductId
) : IRequest<Result<bool>>;
