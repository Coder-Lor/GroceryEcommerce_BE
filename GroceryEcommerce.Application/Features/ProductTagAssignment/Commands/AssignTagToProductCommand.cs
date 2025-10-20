using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductTagAssignment.Commands;

public record AssignTagToProductCommand(
    Guid ProductId,
    Guid TagId
) : IRequest<Result<bool>>;
