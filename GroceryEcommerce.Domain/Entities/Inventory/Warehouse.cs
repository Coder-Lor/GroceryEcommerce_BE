using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Domain.Entities.Inventory;

public class Warehouse
{
    [Key]
    public Guid WarehouseId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(50)]
    public string? Code { get; set; }
    
    public string? Address { get; set; }
    
    [StringLength(100)]
    public string? City { get; set; }
    
    [StringLength(100)]
    public string? State { get; set; }
    
    [StringLength(100)]
    public string? Country { get; set; }
    
    [StringLength(50)]
    public string? Phone { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public ICollection<OrderShipment> OrderShipments { get; set; } = new List<OrderShipment>();
}
