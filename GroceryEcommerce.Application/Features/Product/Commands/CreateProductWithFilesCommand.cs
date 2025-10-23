using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Features.Product.Commands;

public record CreateProductWithFilesCommand(
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
    List<IFormFile>? ImageFiles = null,
    List<string>? ImageAltTexts = null,
    List<int>? ImageDisplayOrders = null,
    List<bool>? ImageIsPrimary = null,
    List<CreateProductVariantRequest>? Variants = null,
    List<CreateProductAttributeValueRequest>? Attributes = null,
    List<Guid>? TagIds = null
) : IRequest<Result<CreateProductResponse>>;
