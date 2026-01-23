namespace GroceryEcommerce.Application.Models.Catalog;

public record ShopDto
{
    public Guid ShopId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public short Status { get; set; }
    public bool IsAccepted { get; set; }
    public Guid OwnerUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? OwnerUserName { get; set; }
    public int ProductCount { get; set; }
    public int OrderCount { get; set; }
}

// Command responses
public record CreateShopResponse : ShopDto;
public record UpdateShopResponse : ShopDto;

// Query responses
public record GetShopByIdResponse : ShopDto;
public record GetShopBySlugResponse : ShopDto;
public record GetShopsByOwnerResponse : ShopDto;


