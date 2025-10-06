namespace GroceryEcommerce.Application.Models.Catalog;

public class ProductVariantDto
{
    public Guid ProductVariantId { get; set; }
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ProductAttributeValueDto> Attributes { get; set; } = new();
}

public class CreateProductVariantRequest
{
    public Guid ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public short Status { get; set; } = 1;
    public List<CreateProductAttributeValueRequest> Attributes { get; set; } = new();
}

public class UpdateProductVariantRequest
{
    public string Sku { get; set; } = string.Empty;
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int StockQuantity { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public short Status { get; set; }
}
