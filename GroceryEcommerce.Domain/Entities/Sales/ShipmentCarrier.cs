using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class ShipmentCarrier
{
    [Key]
    public Guid CarrierId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(50)]
    public string? Code { get; set; }
    
    [StringLength(255)]
    public string? Website { get; set; }
    
    [StringLength(50)]
    public string? Phone { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<OrderShipment> OrderShipments { get; set; } = new List<OrderShipment>();
}
