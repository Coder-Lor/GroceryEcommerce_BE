using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class ShipmentItem
{
    [Key]
    public Guid ShipmentItemId { get; set; }
    
    public Guid ShipmentId { get; set; }
    
    public Guid OrderItemId { get; set; }
    
    public int Quantity { get; set; }
    
    // Navigation properties
    public OrderShipment OrderShipment { get; set; } = null!;
    public OrderItem OrderItem { get; set; } = null!;
}
