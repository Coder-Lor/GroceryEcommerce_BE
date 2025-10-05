using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Cart;

public class Wishlist
{
    [Key]
    public Guid WishlistId { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = "My Wishlist";
    
    public bool IsDefault { get; set; } = true;
    
    public bool IsPublic { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
