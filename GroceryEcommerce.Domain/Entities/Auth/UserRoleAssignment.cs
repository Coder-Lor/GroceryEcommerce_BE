using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class UserRoleAssignment
{
    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }

    [Required]
    [ForeignKey("Role")]
    public Guid RoleId { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey("AssignedByUser")]
    public Guid AssignedBy { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public UserRole? Role { get; set; }
    public User? AssignedByUser { get; set; }
}