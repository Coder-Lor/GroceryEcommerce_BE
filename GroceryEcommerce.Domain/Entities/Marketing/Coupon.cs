using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Marketing;

public class Coupon
{
    [Key]
    public Guid CouponId { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string Code { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public short DiscountType { get; set; } // 1: Percentage, 2: Fixed Amount
    
    [Required]
    public decimal DiscountValue { get; set; }
    
    public decimal? MinOrderAmount { get; set; }
    
    public decimal? MaxDiscountAmount { get; set; }
    
    public int? UsageLimit { get; set; }
    
    public int UsageCount { get; set; } = 0;
    
    public int UserUsageLimit { get; set; } = 1;
    
    public DateTime ValidFrom { get; set; }
    
    public DateTime ValidTo { get; set; }
    
    public short Status { get; set; } = 1;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public User CreatedByUser { get; set; } = null!;
    public User? UpdatedByUser { get; set; }
    public ICollection<CouponUsage> CouponUsages { get; set; } = new List<CouponUsage>();
}
