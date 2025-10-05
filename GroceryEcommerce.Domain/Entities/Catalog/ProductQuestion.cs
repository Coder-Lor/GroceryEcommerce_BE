using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class ProductQuestion
{
    [Key]
    public Guid QuestionId { get; set; } = Guid.NewGuid();
    
    public Guid ProductId { get; set; }
    
    public Guid? UserId { get; set; }
    
    [Required]
    public required string Question { get; set; }
    
    public string? Answer { get; set; }
    
    public Guid? AnsweredBy { get; set; }
    
    public short Status { get; set; } = 1; // 1: Pending, 2: Answered, 3: Hidden
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public User? User { get; set; }
    public User? AnsweredByUser { get; set; }
}
