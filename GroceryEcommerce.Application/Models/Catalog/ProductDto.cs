using GroceryEcommerce.Application.Models.Reviews;

namespace GroceryEcommerce.Application.Models.Catalog;

public record ProductBaseResponse
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public decimal? Cost { get; set; }
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public short Status { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDigital { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductTagDto> Tags { get; set; } = new();
    public decimal? AverageRating { get; set; }
    public int ReviewCount { get; set; }
}

public record ProductDetailDto : ProductBaseResponse
{
    public List<ProductAttributeValueDto> Attributes { get; set; } = new();
    public List<ProductQuestionDto> Questions { get; set; } = new();
    public List<ProductReviewDto> Reviews { get; set; } = new();
}


// product command response 

public record CreateProductResponse : ProductBaseResponse;
public record UpdateProductResponse : ProductBaseResponse;
public record UpdateProductStockResponse : ProductBaseResponse;
public record UpdateProductStatusResponse : ProductBaseResponse;
public record DeleteProductResponse : ProductBaseResponse;

// product query response 

public record GetProductByIdResponse : ProductBaseResponse;
public record GetProductBySkuResponse : ProductBaseResponse;
public record GetProductBySlugResponse : ProductBaseResponse;
public record GetProductsByCategoryResponse : ProductBaseResponse;
public record GetProductsByBrandResponse : ProductBaseResponse;
public record GetProductsByPriceRangeResponse : ProductBaseResponse;
public record GetFeaturedProductsResponse : ProductBaseResponse;
public record GetActiveProductsResponse : ProductBaseResponse;
public record GetLowStockProductsResponse : ProductBaseResponse;
public record SearchProductsResponse : ProductBaseResponse;
public record GetProductsPagingResponse : ProductBaseResponse;

// public record GetProductsByRatingRangeResponse : ProductBaseResponse;
// public record GetProductsByStockQuantityRangeResponse : ProductBaseResponse;
// public record GetProductsByCreatedAtRangeResponse : ProductBaseResponse;
// public record GetProductsByUpdatedAtRangeResponse : ProductBaseResponse;


// public class CreateProductRequest
// {
//     public string Name { get; set; } = string.Empty;
//     public string? Slug { get; set; }
//     public string Sku { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;
//     public string? ShortDescription { get; set; }
//     public decimal Price { get; set; }
//     public decimal? DiscountPrice { get; set; }
//     public decimal? Cost { get; set; }
//     public int StockQuantity { get; set; }
//     public int MinStockLevel { get; set; }
//     public decimal? Weight { get; set; }
//     public string? Dimensions { get; set; }
//     public Guid CategoryId { get; set; }
//     public Guid? BrandId { get; set; }
//     public short Status { get; set; } = 1;
//     public bool IsFeatured { get; set; }
//     public bool IsDigital { get; set; }
//     public string? MetaTitle { get; set; }
//     public string? MetaDescription { get; set; }
//     public List<CreateProductImageRequest> Images { get; set; } = new();
//     public List<CreateProductVariantRequest> Variants { get; set; } = new();
//     public List<Guid> TagIds { get; set; } = new();
// }

// public class UpdateProductRequest
// {
//     public string Name { get; set; } = string.Empty;
//     public string? Slug { get; set; }
//     public string Sku { get; set; } = string.Empty;
//     public string Description { get; set; } = string.Empty;
//     public string? ShortDescription { get; set; }
//     public decimal Price { get; set; }
//     public decimal? DiscountPrice { get; set; }
//     public decimal? Cost { get; set; }
//     public int StockQuantity { get; set; }
//     public int MinStockLevel { get; set; }
//     public decimal? Weight { get; set; }
//     public string? Dimensions { get; set; }
//     public Guid CategoryId { get; set; }
//     public Guid? BrandId { get; set; }
//     public short Status { get; set; }
//     public bool IsFeatured { get; set; }
//     public bool IsDigital { get; set; }
//     public string? MetaTitle { get; set; }
//     public string? MetaDescription { get; set; }
// }


