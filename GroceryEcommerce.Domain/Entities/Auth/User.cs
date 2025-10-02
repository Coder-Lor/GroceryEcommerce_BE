using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string PasswordHash { get; set; }
    
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }
   
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    public short Status { get; set; } = 1; // 1: Active, 0: Inactive, -1: Banned
    public bool EmailVerified { get; set; } = false;
    public bool PhoneVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}