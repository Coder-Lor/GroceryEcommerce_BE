using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Domain.Entities.Cart;

public class ShoppingCartItem
{
    [Key]
    public Guid CartItemId { get; set; }
    
    public Guid CartId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid? ProductVariantId { get; set; }
    
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public ShoppingCart ShoppingCart { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ProductVariant? ProductVariant { get; set; }
}
