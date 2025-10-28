namespace GroceryEcommerce.Application.Models.Cart;

public class WishlistDto
{
    public Guid WishlistId { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<WishlistItemDto> Items { get; set; } = new();
}

public class WishlistItemDto
{
    public Guid WishlistItemId { get; set; }
    public Guid WishlistId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal? ProductDiscountPrice { get; set; }
    public int ProductStockQuantity { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddToWishlistRequest
{
    public Guid ProductId { get; set; }
    public Guid? VariantId { get; set; }
}
