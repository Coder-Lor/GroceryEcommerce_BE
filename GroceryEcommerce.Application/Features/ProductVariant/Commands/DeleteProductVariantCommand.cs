using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductVariant.Commands;

public record DeleteProductVariantCommand(
    Guid VariantId
) : IRequest<Result<bool>>;
