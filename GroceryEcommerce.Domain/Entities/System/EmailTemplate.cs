using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.System;

public class EmailTemplate
{
    [Key]
    public Guid TemplateId { get; set; }
    
    [Required]
    [StringLength(255)]
    public required string Name { get; set; }
    
    [StringLength(500)]
    public string? Subject { get; set; }
    
    public string? Body { get; set; }
    
    [StringLength(10)]
    public string Language { get; set; } = "en";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
