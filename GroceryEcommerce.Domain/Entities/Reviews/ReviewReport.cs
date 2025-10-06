using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Reviews;

public class ReviewReport
{
    [Key]
    public Guid ReportId { get; set; }
    
    public Guid ReviewId { get; set; }
    
    public Guid UserId { get; set; }
    
    [StringLength(500)]
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool Processed { get; set; } = false;
    
    public DateTime? ProcessedAt { get; set; }
    
    public Guid? ProcessedBy { get; set; }
    
    // Navigation properties
    public ProductReview ProductReview { get; set; } = null!;
    public User User { get; set; } = null!;
    public User? ProcessedByUser { get; set; }
}
