using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductImage
{
    [Key]
    public Guid ImageId { get; set; } = Guid.NewGuid();
    
    public Guid ProductId { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string ImageUrl { get; set; }
    
    [StringLength(255)]
    public string? AltText { get; set; }
    
    public int DisplayOrder { get; set; } = 0;
    
    public bool IsPrimary { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Product Product { get; set; } = null!;
}
