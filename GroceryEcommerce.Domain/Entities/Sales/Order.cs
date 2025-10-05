using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class Order
{
    [Key]
    public Guid OrderId { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string OrderNumber { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public decimal SubTotal { get; set; }
    
    public decimal TaxAmount { get; set; } = 0;
    
    public decimal ShippingAmount { get; set; } = 0;
    
    public decimal DiscountAmount { get; set; } = 0;
    
    [Required]
    public decimal TotalAmount { get; set; }
    
    public short Status { get; set; } = 1; // 1: Pending, 2: Processing, 3: Shipped, 4: Delivered, 5: Cancelled
    
    public short PaymentStatus { get; set; } = 1; // 1: Pending, 2: Paid, 3: Failed, 4: Refunded
    
    public short PaymentMethod { get; set; } // 1: Credit Card, 2: PayPal, 3: Bank Transfer, 4: COD
    
    // Shipping
    [Required]
    [StringLength(100)]
    public required string ShippingFirstName { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ShippingLastName { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string ShippingEmail { get; set; }
    
    [StringLength(20)]
    public string? ShippingPhone { get; set; }
    
    [Required]
    public required string ShippingAddress { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ShippingCity { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ShippingState { get; set; }
    
    [Required]
    [StringLength(20)]
    public required string ShippingZipCode { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ShippingCountry { get; set; }
    
    // Billing
    [Required]
    [StringLength(100)]
    public required string BillingFirstName { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string BillingLastName { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string BillingEmail { get; set; }
    
    [StringLength(20)]
    public string? BillingPhone { get; set; }
    
    [Required]
    public required string BillingAddress { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string BillingCity { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string BillingState { get; set; }
    
    [Required]
    [StringLength(20)]
    public required string BillingZipCode { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string BillingCountry { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime? EstimatedDeliveryDate { get; set; }
    
    public DateTime? DeliveredAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? CreatedBy { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
    public ICollection<OrderPayment> OrderPayments { get; set; } = new List<OrderPayment>();
    public ICollection<OrderShipment> OrderShipments { get; set; } = new List<OrderShipment>();
    public ICollection<OrderRefund> OrderRefunds { get; set; } = new List<OrderRefund>();
}
