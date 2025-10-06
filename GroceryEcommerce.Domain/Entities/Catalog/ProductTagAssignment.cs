using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductTagAssignment
{
    [Key]
    [Column(Order = 0)]
    public Guid ProductId { get; set; }
    
    [Key]
    [Column(Order = 1)]
    public Guid TagId { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ProductTag ProductTag { get; set; } = null!;
}
