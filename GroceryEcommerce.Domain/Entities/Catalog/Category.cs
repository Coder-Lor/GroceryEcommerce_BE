using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class Category
{
    [Key]
    public Guid CategoryId { get; set; } = Guid.NewGuid();
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(255)]
    public string? Slug { get; set; }
    
    public string? Description { get; set; }
    
    [StringLength(500)]
    public string? ImageUrl { get; set; }
    
    public Guid? ParentCategoryId { get; set; }
    
    public int DisplayOrder { get; set; } = 0;
    
    public short Status { get; set; } = 1;
    
    [StringLength(255)]
    public string? MetaTitle { get; set; }
    
    [StringLength(500)]
    public string? MetaDescription { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public User CreatedByUser { get; set; } = null!;
    public User? UpdatedByUser { get; set; }
}
