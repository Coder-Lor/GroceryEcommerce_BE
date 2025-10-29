using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Commands;

public record DeleteProductTagCommand(
    Guid TagId
) : IRequest<Result<bool>>;
