using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;

public record CreateProductVariantCommand(
    Guid ProductId,
    string Sku,
    string Name,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    int MinStockLevel,
    decimal? Weight,
    string? Dimensions,
    string? ImageUrl,
    short Status = 1
) : IRequest<Result<bool>>;
