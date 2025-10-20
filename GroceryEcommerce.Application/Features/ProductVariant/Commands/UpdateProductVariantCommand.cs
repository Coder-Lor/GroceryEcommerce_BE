using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductVariant.Commands;

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
    short Status
) : IRequest<Result<UpdateProductVariantResponse>>;
