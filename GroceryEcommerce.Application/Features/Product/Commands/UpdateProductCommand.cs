using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Commands;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string? Slug,
    string Sku,
    string Description,
    string? ShortDescription,
    decimal Price,
    decimal? DiscountPrice,
    decimal? Cost,
    int StockQuantity,
    int MinStockLevel,
    decimal? Weight,
    string? Dimensions,
    Guid CategoryId,
    Guid? BrandId,
    short Status,
    bool IsFeatured,
    bool IsDigital,
    string? MetaTitle,
    string? MetaDescription
) : IRequest<Result<UpdateProductResponse>>;