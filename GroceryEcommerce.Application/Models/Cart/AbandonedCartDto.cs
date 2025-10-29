namespace GroceryEcommerce.Application.Models.Cart;

public class AbandonedCartDto
{
    public Guid AbandonedCartId { get; set; }
    public Guid UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public decimal CartValue { get; set; }
    public int ItemCount { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RecoveredAt { get; set; }
    public bool IsRecovered { get; set; }
    public List<ShoppingCartItemDto> Items { get; set; } = new();
}

public class MarkCartsNotifiedRequest
{
    public List<Guid> AbandonedCartIds { get; set; } = new();
}
