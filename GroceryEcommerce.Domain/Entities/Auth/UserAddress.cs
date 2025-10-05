using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class UserAddress
{
    [Key]
    public Guid AddressId { get; set; }
    
    public Guid UserId { get; set; }
    
    public short AddressType { get; set; } // 1: Home, 2: Office, 3: Other
    
    [Required]
    [StringLength(255)]
    public required string AddressLine1 { get; set; }
    
    [StringLength(255)]
    public string? AddressLine2 { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string City { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string State { get; set; }
    
    [Required]
    [StringLength(20)]
    public required string ZipCode { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Country { get; set; }
    
    public bool IsDefault { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation property
    public User User { get; set; } = null!;
}