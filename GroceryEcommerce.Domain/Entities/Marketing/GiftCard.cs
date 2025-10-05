using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Marketing;

public class GiftCard
{
    [Key]
    public Guid GiftCardId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Code { get; set; }
    
    [Required]
    public decimal InitialAmount { get; set; }
    
    [Required]
    public decimal Balance { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? CreatedBy { get; set; }
    
    // Navigation properties
    public User? CreatedByUser { get; set; }
}
