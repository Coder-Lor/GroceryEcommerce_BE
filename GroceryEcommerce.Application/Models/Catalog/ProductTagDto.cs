namespace GroceryEcommerce.Application.Models.Catalog;

public class ProductTagDto
{
    public Guid ProductTagId { get; set; }
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ProductCount { get; set; }
}

public class CreateProductTagRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class UpdateProductTagRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
}
