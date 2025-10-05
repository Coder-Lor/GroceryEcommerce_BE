using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Cart;

public class AbandonedCart
{
    [Key]
    public Guid AbandonedId { get; set; }
    
    public Guid CartId { get; set; }
    
    public Guid? UserId { get; set; }
    
    public DateTime AbandonedAt { get; set; } = DateTime.UtcNow;
    
    public bool Notified { get; set; } = false;
    
    // Navigation properties
    public ShoppingCart Cart { get; set; } = null!;
    public User? User { get; set; }
}