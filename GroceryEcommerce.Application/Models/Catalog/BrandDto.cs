namespace GroceryEcommerce.Application.Models.Catalog;

// create instance dto for brand by query command respone

public record BrandBaseResponse
{
    public Guid BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// for command respone

public record CreateBrandResponse : BrandBaseResponse;
public record UpdateBrandResponse : BrandBaseResponse;

// for query response

public record BrandDto : BrandBaseResponse
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ProductCount { get; set; }
}

public record GetBrandByIdResponse : BrandDto;
public record GetBrandByNameResponse : BrandDto;
public record GetBrandBySlugResponse : BrandDto;