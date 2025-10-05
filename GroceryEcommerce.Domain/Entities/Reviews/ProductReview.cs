using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Domain.Entities.Reviews;

public class ProductReview
{
    [Key]
    public Guid ReviewId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid? OrderId { get; set; }
    
    [Required]
    public short Rating { get; set; } // 1-5
    
    [StringLength(255)]
    public string? Title { get; set; }
    
    public string? Content { get; set; }
    
    public bool IsVerifiedPurchase { get; set; } = false;
    
    public short Status { get; set; } = 1; // 1: Pending, 2: Approved, 3: Rejected
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
    public Order? Order { get; set; }
    public ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();
    public ICollection<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();
    public ICollection<ReviewReport> ReviewReports { get; set; } = new List<ReviewReport>();
}
