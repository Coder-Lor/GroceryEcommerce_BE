namespace GroceryEcommerce.Application.Models;

public class AuditLogDto
{
    public Guid AuditId { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Entity { get; set; }
    public Guid? EntityId { get; set; }
    public string? Detail { get; set; }
    public DateTime CreatedAt { get; set; }
}
