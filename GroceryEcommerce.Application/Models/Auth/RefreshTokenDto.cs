namespace GroceryEcommerce.Application.Models;

public class RefreshTokenDto
{
    public Guid TokenId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string RefreshTokenValue { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
}
