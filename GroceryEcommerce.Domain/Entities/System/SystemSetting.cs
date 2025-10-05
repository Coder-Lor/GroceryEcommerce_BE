using System.ComponentModel.DataAnnotations;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Domain.Entities.System;

public class SystemSetting
{
    [Key]
    public Guid SettingId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string SettingKey { get; set; }
    
    public string? SettingValue { get; set; }
    
    public short SettingType { get; set; } // 1: String, 2: Number, 3: Boolean, 4: JSON
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [StringLength(50)]
    public string? Category { get; set; }
    
    public bool IsPublic { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    public Guid? UpdatedBy { get; set; }
    
    // Navigation properties
    public User? UpdatedByUser { get; set; }
}
