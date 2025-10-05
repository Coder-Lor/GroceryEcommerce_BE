using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Domain.Entities.System;

public class Currency
{
    [Key]
    [StringLength(10)]
    public required string CurrencyCode { get; set; }
    
    [StringLength(10)]
    public string? Symbol { get; set; }
    
    [StringLength(50)]
    public string? Name { get; set; }
    
    public decimal? ExchangeRate { get; set; } // relative to base currency
    
    public DateTime? UpdatedAt { get; set; }
}
