using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Inventory;

public class PurchaseOrder
{
    [Key]
    public Guid PurchaseOrderId { get; set; }
    
    public Guid SupplierId { get; set; }
    
    public Guid? WarehouseId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string OrderNumber { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpectedDate { get; set; }
    
    public short Status { get; set; } = 1; // 1: Created, 2: Ordered, 3: Received, 4: Cancelled
    
    public decimal TotalAmount { get; set; } = 0;
    
    public Guid? CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Supplier Supplier { get; set; } = null!;
    public User? CreatedByUser { get; set; }
    public Warehouse? Warehouse { get; set; }
    public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
