using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class AuditLog
{
    [Key]
    public Guid AuditId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string Action { get; set; }
    
    [StringLength(100)]
    public string? Entity { get; set; }
    
    public Guid? EntityId { get; set; }
    
    public string? Detail { get; set; } // JSON
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public User? User { get; set; }
}