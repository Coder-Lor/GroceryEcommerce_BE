using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Marketing;

public class RewardPoint
{
    [Key]
    public Guid RewardId { get; set; }
    
    public Guid UserId { get; set; }
    
    public int Points { get; set; }
    
    [StringLength(255)]
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
}
