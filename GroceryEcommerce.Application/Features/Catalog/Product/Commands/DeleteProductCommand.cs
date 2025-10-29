using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Commands;

public record DeleteProductCommand(
    Guid ProductId
) : IRequest<Result<bool>>;
