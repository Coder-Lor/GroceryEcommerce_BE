using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductAttributeValue
{
    [Key]
    public Guid ValueId { get; set; } = Guid.NewGuid();
    
    public Guid ProductId { get; set; }
    
    public Guid AttributeId { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string Value { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ProductAttribute Attribute { get; set; } = null!;
}
