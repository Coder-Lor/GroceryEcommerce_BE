using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Domain.Entities.Inventory;

public class StockMovement
{
    [Key]
    public Guid MovementId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid? ProductVariantId { get; set; }
    
    public Guid? WarehouseId { get; set; }
    
    public short MovementType { get; set; } // 1: In, 2: Out, 3: Adjustment
    
    public int Quantity { get; set; }
    
    public int PreviousStock { get; set; }
    
    public int NewStock { get; set; }
    
    [StringLength(255)]
    public string? Reason { get; set; }
    
    public Guid? ReferenceId { get; set; }
    
    public short? ReferenceType { get; set; } // 1: Order, 2: Purchase, 3: Adjustment
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public ProductVariant? ProductVariant { get; set; }
    public Warehouse? Warehouse { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
