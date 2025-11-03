using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroceryEcommerce.API.Contracts.Catalog;

public class CreateProductVariantForm
{
    [FromForm]
    public Guid ProductId { get; set; }
    [FromForm]
    public string Sku { get; set; } = string.Empty;
    [FromForm]
    public string VariantName { get; set; } = string.Empty;
    [FromForm]
    public decimal Price { get; set; }
    [FromForm]
    public int StockQuantity { get; set; }
    [FromForm]
    public int MinStockLevel { get; set; }
    [FromForm]
    public short Status { get; set; }
    [FromForm]
    public decimal? DiscountPrice { get; set; }
    [FromForm]
    public decimal? Weight { get; set; }
    [FromForm]
    public string? Dimensions { get; set; }
    [FromForm]
    public IFormFile? ImageFile { get; set; }
}


