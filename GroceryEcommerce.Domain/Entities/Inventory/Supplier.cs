using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Inventory;

public class Supplier
{
    [Key]
    public Guid SupplierId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(255)]
    public string? ContactName { get; set; }
    
    [StringLength(255)]
    public string? ContactEmail { get; set; }
    
    [StringLength(50)]
    public string? ContactPhone { get; set; }
    
    public string? Address { get; set; }
    
    public string? Note { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
