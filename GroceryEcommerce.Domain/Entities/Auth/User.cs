using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.Auth;

public class User
{
    [Key]
    public Guid UserId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Email { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string PasswordHash { get; set; }
    
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }
   
    [StringLength(20)]
    public string? PhoneNumber { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string Username { get; set; }
    
    public short Status { get; set; } = 1; // 1: Active, 0: Inactive, -1: Banned
    public bool EmailVerified { get; set; } = false;
    public bool PhoneVerified { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LastFailedLogin { get; set; }
    public DateTime? LockedUntil { get; set; }
    
    
    // Navigation properties
    public ICollection<UserRoleAssignment> UserRoleAssignments { get; set; } = new List<UserRoleAssignment>();
    public ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
    public ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Cart.ShoppingCart> ShoppingCarts { get; set; } = new List<Cart.ShoppingCart>();
    public ICollection<Cart.Wishlist> Wishlists { get; set; } = new List<Cart.Wishlist>();
    public ICollection<Cart.AbandonedCart> AbandonedCarts { get; set; } = new List<Cart.AbandonedCart>();
    public ICollection<Sales.Order> Orders { get; set; } = new List<Sales.Order>();
    public ICollection<Marketing.CouponUsage> CouponUsages { get; set; } = new List<Marketing.CouponUsage>();
    public ICollection<Marketing.RewardPoint> RewardPoints { get; set; } = new List<Marketing.RewardPoint>();
    public ICollection<Reviews.ProductReview> ProductReviews { get; set; } = new List<Reviews.ProductReview>();
}