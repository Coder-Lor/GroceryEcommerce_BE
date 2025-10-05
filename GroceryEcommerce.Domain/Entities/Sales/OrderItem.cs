using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class OrderItem
{
    [Key]
    public Guid OrderItemId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid? ProductVariantId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string ProductName { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string ProductSku { get; set; }
    
    public int Quantity { get; set; }
    
    [Required]
    public decimal UnitPrice { get; set; }
    
    [Required]
    public decimal TotalPrice { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ProductVariant? ProductVariant { get; set; }
    public ICollection<ShipmentItem> ShipmentItems { get; set; } = new List<ShipmentItem>();
}
