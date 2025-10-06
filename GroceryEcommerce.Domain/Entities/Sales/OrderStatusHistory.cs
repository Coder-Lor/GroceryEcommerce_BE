using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Sales;

public class OrderStatusHistory
{
    [Key]
    public Guid HistoryId { get; set; }
    
    public Guid OrderId { get; set; }
    
    public short? FromStatus { get; set; }
    
    [Required]
    public short ToStatus { get; set; }
    
    [StringLength(500)]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    
    // Navigation properties
    public Order Order { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
    
    // Computed properties for mapper
    public short? OldStatus => FromStatus;
    public short NewStatus => ToStatus;
    public User? ChangedByUser => CreatedByUser;
}
