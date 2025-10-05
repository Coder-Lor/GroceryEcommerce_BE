using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class UserRoleAssignment
{
    [Key]
    [Column(Order = 0)]
    public Guid UserId { get; set; }
    
    [Key]
    [Column(Order = 1)]
    public Guid RoleId { get; set; }
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    
    public Guid AssignedBy { get; set; }
    
    // Navigation properties
    public User User { get; set; } = null!;
    public UserRole Role { get; set; } = null!;
    public User AssignedByUser { get; set; } = null!;
}