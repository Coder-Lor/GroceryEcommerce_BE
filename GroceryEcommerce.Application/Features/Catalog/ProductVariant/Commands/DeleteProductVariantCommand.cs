using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;

public record DeleteProductVariantCommand(
    Guid VariantId
) : IRequest<Result<bool>>;
