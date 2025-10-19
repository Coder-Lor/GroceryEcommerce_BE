namespace GroceryEcommerce.Application.Models.Catalog;

public class ProductTagAssignmentDto
{
    public Guid ProductId { get; set; }
    public Guid TagId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
