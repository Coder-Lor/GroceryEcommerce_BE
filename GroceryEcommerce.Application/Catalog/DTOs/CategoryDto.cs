namespace GroceryEcommerce.Application.Catalog.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentName { get; set; }
        public int DisplayOrder { get; set; }
        public short Status { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CategoryDto> Children { get; set; } = new();
        public int ProductCount { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int DisplayOrder { get; set; }
        public short Status { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}