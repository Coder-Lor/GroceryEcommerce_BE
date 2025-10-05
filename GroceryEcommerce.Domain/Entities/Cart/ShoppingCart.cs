using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Cart;

public class ShoppingCart
{
    [Key]
    public Guid CartId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [StringLength(255)]
    public string? SessionId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
