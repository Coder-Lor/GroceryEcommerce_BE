using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Domain.Entities.Inventory;

public class PurchaseOrderItem
{
    [Key]
    public Guid PoiId { get; set; }
    
    public Guid PurchaseOrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid? VariantId { get; set; }
    
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitCost { get; set; }
    
    [Required]
    public decimal TotalCost { get; set; }
    
    // Navigation properties
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ProductVariant? ProductVariant { get; set; }
}
