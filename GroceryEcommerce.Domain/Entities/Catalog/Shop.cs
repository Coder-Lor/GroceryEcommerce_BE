using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Domain.Entities.Catalog;

public class Shop
{
    [Key]
    public Guid ShopId { get; set; } = Guid.NewGuid();

    [Required]
    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(255)]
    public string? Slug { get; set; }

    public string? Description { get; set; }

    [StringLength(500)]
    public string? LogoUrl { get; set; }

    public short Status { get; set; } = 1;

    public bool IsAccepted { get; set; } = false;

    public Guid OwnerUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public User OwnerUser { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}


