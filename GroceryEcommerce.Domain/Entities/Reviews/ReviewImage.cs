using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Reviews;

public class ReviewImage
{
    [Key]
    public Guid ImageId { get; set; }
    
    public Guid ReviewId { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ProductReview Review { get; set; } = null!;
}
