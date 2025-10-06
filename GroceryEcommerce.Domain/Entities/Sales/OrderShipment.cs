using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class OrderShipment
{
    [Key]
    public Guid ShipmentId { get; set; }
    
    public Guid OrderId { get; set; }
    
    [StringLength(100)]
    public string? ShipmentNumber { get; set; }
    
    public Guid? CarrierId { get; set; }
    
    [StringLength(200)]
    public string? TrackingNumber { get; set; }
    
    public DateTime? ShippedAt { get; set; }
    
    public DateTime? DeliveredAt { get; set; }
    
    public short Status { get; set; } = 1; // 1: Ready, 2: Shipped, 3: In Transit, 4: Delivered, 5: Returned
    
    public decimal ShippingCost { get; set; } = 0;
    
    public Guid? WarehouseId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? ShippedBy { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public ShipmentCarrier? ShipmentCarrier { get; set; }
    public Warehouse? Warehouse { get; set; }
    public User? ShippedByUser { get; set; }
    public ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
}
