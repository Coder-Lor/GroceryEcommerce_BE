using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Commands;

public record UpdateProductWithFilesCommand : IRequest<Result<UpdateProductResponse>>
{
    public Guid ProductId { get; set; }
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public required string Sku { get; set; }
    public required string Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal? Cost { get; set; }
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid? ShopId { get; set; }
    public short Status { get; set; } = 1;
    public bool IsFeatured { get; set; } = false;
    public bool IsDigital { get; set; } = false;
    public string? MetaTitle { get; set; } = null;
    public string? MetaDescription { get; set; } = null;
    
    // Image management
    public List<IFormFile>? NewImageFiles { get; set; } = null;
    public List<string>? NewImageAltTexts { get; set; } = null;
    public List<int>? NewImageDisplayOrders { get; set; } = null;
    public List<bool>? NewImageIsPrimary { get; set; } = null;
    
    // IDs của các hình ảnh cần xóa
    public List<Guid>? ImageIdsToDelete { get; set; } = null;
    
    // Product variants, attributes, tags
    public List<CreateProductVariantRequest>? Variants { get; set; } = null;
    public List<CreateProductAttributeValueRequest>? Attributes { get; set; } = null;
    public List<Guid>? TagIds { get; set; } = null;
}

