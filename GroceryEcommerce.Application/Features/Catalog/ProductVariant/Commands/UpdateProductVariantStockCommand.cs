using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;

public record UpdateProductVariantStockCommand(
    Guid VariantId,
    int Quantity
) : IRequest<Result<bool>>;
