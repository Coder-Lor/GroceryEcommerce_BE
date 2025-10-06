using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.Reviews;

public class ReviewVote
{
    [Key]
    public Guid VoteId { get; set; }
    
    public Guid ReviewId { get; set; }
    
    public Guid UserId { get; set; }
    
    public bool Helpful { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ProductReview ProductReview { get; set; } = null!;
    public User User { get; set; } = null!;
}
