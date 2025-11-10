using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductVariant
{
    [Key]
    public Guid VariantId { get; set; } = Guid.NewGuid();
    
    public Guid ProductId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Sku { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public decimal? DiscountPrice { get; set; }
    
    public int StockQuantity { get; set; } = 0;
    
    public int MinStockLevel { get; set; } = 0;
    
    public decimal? Weight { get; set; }
    
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    
    public short Status { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<ProductAttributeValue> VariantAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
