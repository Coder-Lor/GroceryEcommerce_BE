using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Domain.Entities.Marketing;

public class CouponUsage
{
    [Key]
    public Guid UsageId { get; set; }
    
    public Guid CouponId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid OrderId { get; set; }
    
    [Required]
    public decimal DiscountAmount { get; set; }
    
    public DateTime UsedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Coupon Coupon { get; set; } = null!;
    public User User { get; set; } = null!;
    public Order Order { get; set; } = null!;
}
