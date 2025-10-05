using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Domain.Entities.Cart;

public class WishlistItem
{
    [Key]
    public Guid WishlistItemId { get; set; }
    
    public Guid WishlistId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid? ProductVariantId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Wishlist Wishlist { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ProductVariant? ProductVariant { get; set; }
}
