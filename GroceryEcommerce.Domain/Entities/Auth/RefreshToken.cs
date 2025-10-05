using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class RefreshToken
{
    [Key]
    public Guid TokenId { get; set; }
    
    public Guid UserId { get; set; }
    
    [Required]
    public required string RefreshTokenValue { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool Revoked { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [StringLength(45)]
    public string? CreatedByIp { get; set; }
    
    public string? ReplacedByToken { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}