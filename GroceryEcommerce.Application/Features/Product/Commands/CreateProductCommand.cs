using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Product.Commands;

public record CreateProductCommand(
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
    short Status = 1,
    bool IsFeatured = false,
    bool IsDigital = false,
    string? MetaTitle = null,
    string? MetaDescription = null,
    List<CreateProductImageRequest> Images = null,
    List<CreateProductVariantRequest> Variants = null,
    List<CreateProductAttributeValueRequest> Attributes = null,
    List<Guid> TagIds = null
) : IRequest<Result<CreateProductResponse>>;