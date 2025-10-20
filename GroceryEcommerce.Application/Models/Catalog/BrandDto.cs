namespace GroceryEcommerce.Application.Models.Catalog;

public record BrandDto
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
    public int ProductCount { get; set; }
}

// for command responses
public record CreateBrandResponse : BrandDto;
public record UpdateBrandResponse : BrandDto;

// for query responses
public record GetBrandByIdResponse : BrandDto;
public record GetBrandByNameResponse : BrandDto;
public record GetBrandBySlugResponse : BrandDto;