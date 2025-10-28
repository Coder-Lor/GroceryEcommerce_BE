using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductImage.Commands;

public record DeleteProductImageCommand(
    Guid ImageId
) : IRequest<Result<bool>>;
