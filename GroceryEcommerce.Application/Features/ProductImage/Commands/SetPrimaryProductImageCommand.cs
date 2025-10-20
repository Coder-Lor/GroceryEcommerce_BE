using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductImage.Commands;

public record SetPrimaryProductImageCommand(
    Guid ImageId
) : IRequest<Result<bool>>;


