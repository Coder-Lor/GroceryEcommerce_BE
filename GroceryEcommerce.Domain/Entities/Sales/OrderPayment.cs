using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class OrderPayment
{
    [Key]
    public Guid PaymentId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public short PaymentMethod { get; set; } // mirror payment_method in orders
    
    [StringLength(200)]
    public string? TransactionId { get; set; }
    
    public string? GatewayResponse { get; set; } // JSON
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    [StringLength(10)]
    public string Currency { get; set; } = "USD";
    
    public short Status { get; set; } = 1; // 1: Pending, 2: Completed, 3: Failed, 4: Refunded
    
    public DateTime? PaidAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? ProcessedBy { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public User? ProcessedByUser { get; set; }
    public ICollection<OrderRefund> OrderRefunds { get; set; } = new List<OrderRefund>();
}
