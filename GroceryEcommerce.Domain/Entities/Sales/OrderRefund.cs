using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class OrderRefund
{
    [Key]
    public Guid RefundId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public Guid? PaymentId { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [StringLength(500)]
    public string? Reason { get; set; }
    
    public short Status { get; set; } = 1; // 1: Requested, 2: Processed, 3: Rejected
    
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ProcessedAt { get; set; }
    
    public Guid? ProcessedBy { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public OrderPayment? Payment { get; set; }
    public User? ProcessedByUser { get; set; }
}
