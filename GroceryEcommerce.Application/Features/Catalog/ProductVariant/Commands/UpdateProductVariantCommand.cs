using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Commands;

public record UpdateProductVariantCommand(
    Guid VariantId,
    string Sku,
    string Name,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity,
    int MinStockLevel,
    decimal? Weight,
    string? Dimensions,
    string? ImageUrl,
    short Status
) : IRequest<Result<bool>>;
