using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class UserRole
{
    [Key]
    public Guid RoleId { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string RoleName { get; set; }
    
    [StringLength(255)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}