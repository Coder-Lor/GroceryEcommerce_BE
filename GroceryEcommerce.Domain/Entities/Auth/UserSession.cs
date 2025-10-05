using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class UserSession
{
    [Key]
    public Guid SessionId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string SessionToken { get; set; }
    
    public string? DeviceInfo { get; set; }
    
    [StringLength(45)]
    public string? IpAddress { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ExpiresAt { get; set; }
    
    public bool Revoked { get; set; } = false;
    
    // Navigation property
    public User? User { get; set; }
}