using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductTag
{
    [Key]
    public Guid TagId { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [StringLength(150)]
    public string? Slug { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<ProductTagAssignment> ProductTagAssignments { get; set; } = new List<ProductTagAssignment>();
}
