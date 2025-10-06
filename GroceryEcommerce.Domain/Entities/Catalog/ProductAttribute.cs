using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductAttribute
{
    [Key]
    public Guid AttributeId { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string DisplayName { get; set; }
    
    public short AttributeType { get; set; } // 1: Text, 2: Number, 3: Boolean, 4: Select, 5: Color
    
    public bool IsRequired { get; set; } = false;
    
    public int DisplayOrder { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
