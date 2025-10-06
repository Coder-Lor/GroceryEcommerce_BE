namespace GroceryEcommerce.Application.Models;

public class UserRoleAssignmentDto
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public Guid AssignedBy { get; set; }
}