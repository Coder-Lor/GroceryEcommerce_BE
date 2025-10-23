namespace GroceryEcommerce.Application.Models.Catalog;


public record  CategoryDto {
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int DisplayOrder { get; set; }
    public short Status { get; set; } = 1;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();
    public int ProductCount { get; set; }
}

public record CreateCategoryResponse : CategoryDto;
public record UpdateCategoryResponse : CategoryDto;
public record DeleteCategoryResponse : CategoryDto;
public record GetCategoryByIdResponse : CategoryDto;
public record GetCategoriesResponse : CategoryDto;
public record GetCategoriesByParentIdResponse : CategoryDto;
public record GetCategoriesByStatusResponse : CategoryDto;
public record GetCategoriesBySearchResponse : CategoryDto;
public record GetActiveCategoriesResponse : CategoryDto;
public record GetCategoryByNameResponse : CategoryDto;

public record GetCategoryTreeResponse
{
    public List<CategoryDto> Categories { get; set; } = new();
}
public record GetCategoryPathResponse : CategoryDto;
public record GetRootCategoriesResponse : CategoryDto;
public record GetCategoryBySlugResponse : CategoryDto;
public record GetSubCategoriesResponse : CategoryDto;
public record SearchCategoriesByNameResponse : CategoryDto;

// Add lightweight aliases and request DTOs expected by older code (ICatalogService and mapping)

public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public short Status { get; set; } = 1;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public short Status { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}
