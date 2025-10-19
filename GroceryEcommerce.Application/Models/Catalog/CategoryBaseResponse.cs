namespace GroceryEcommerce.Application.Models.Catalog;


public record  CategoryBaseResponse {
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
    public List<CategoryBaseResponse> SubCategories { get; set; } = new();
    public int ProductCount { get; set; }
}

public record CreateCategoryResponse : CategoryBaseResponse;
public record UpdateCategoryResponse : CategoryBaseResponse;
public record DeleteCategoryResponse : CategoryBaseResponse;
public record GetCategoryByIdResponse : CategoryBaseResponse;
public record GetCategoriesResponse : CategoryBaseResponse;
public record GetCategoriesByParentIdResponse : CategoryBaseResponse;
public record GetCategoriesByStatusResponse : CategoryBaseResponse;
public record GetCategoriesBySearchResponse : CategoryBaseResponse;
public record GetActiveCategoriesResponse : CategoryBaseResponse;
public record GetCategoryByNameResponse : CategoryBaseResponse;
public record GetCategoryTreeResponse : CategoryBaseResponse;
public record GetCategoryPathResponse : CategoryBaseResponse;
public record GetRootCategoriesResponse : CategoryBaseResponse;
public record GetCategoryBySlugResponse : CategoryBaseResponse;
public record GetSubCategoriesResponse : CategoryBaseResponse;
public record SearchCategoriesByNameResponse : CategoryBaseResponse;

// Add lightweight aliases and request DTOs expected by older code (ICatalogService and mapping)
public record CategoryDto : CategoryBaseResponse;

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
