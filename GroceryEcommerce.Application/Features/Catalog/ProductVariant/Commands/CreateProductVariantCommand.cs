using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
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
    short Status = 1
) : IRequest<Result<CreateProductVariantResponse>>;
