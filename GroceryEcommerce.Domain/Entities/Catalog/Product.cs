using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class Product
{
    [Key]
    public Guid ProductId { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(255)]
    public string? Slug { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Sku { get; set; }
    
    [Required]
    public required string Description { get; set; }
    
    [StringLength(500)]
    public string? ShortDescription { get; set; }
    
    [Required]
    public decimal Price { get; set; }
    
    public decimal? DiscountPrice { get; set; }
    
    public decimal? Cost { get; set; }
    
    public int StockQuantity { get; set; } = 0;
    
    public int MinStockLevel { get; set; } = 0;
    
    public decimal? Weight { get; set; }
    
    [StringLength(50)]
    public string? Dimensions { get; set; }
    
    public Guid CategoryId { get; set; }
    
    public Guid? BrandId { get; set; }
    
    public short Status { get; set; } = 1;
    
    public bool IsFeatured { get; set; } = false;
    
    public bool IsDigital { get; set; } = false;
    
    [StringLength(255)]
    public string? MetaTitle { get; set; }
    
    [StringLength(500)]
    public string? MetaDescription { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public Category Category { get; set; } = null!;
    public Brand? Brand { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public User? UpdatedByUser { get; set; }
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
    public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductTagAssignment> ProductTagAssignments { get; set; } = new List<ProductTagAssignment>();
    public ICollection<ProductQuestion> ProductQuestions { get; set; } = new List<ProductQuestion>();
    public ICollection<Reviews.ProductReview> Reviews { get; set; } = new List<Reviews.ProductReview>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
