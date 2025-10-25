using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Features.Product.Commands;

public record CreateProductWithFilesCommand: IRequest<Result<CreateProductResponse>> 
{
    public string Name { get; set;}
    public string? Slug { get; set;}
    public string Sku { get; set;}
    public string Description { get; set;}
    public string? ShortDescription { get; set;}
    public decimal Price { get; set;}
    public decimal? DiscountPrice { get; set;}
    public decimal? Cost { get; set;}
    public int StockQuantity { get; set;}
    public int MinStockLevel { get; set;}
    public decimal? Weight { get; set;}
    public string? Dimensions { get; set;}
    public Guid CategoryId { get; set;}
    public Guid? BrandId { get; set;}
    public short Status { get; set; } = 1;
    public bool IsFeatured { get; set; } = false;
    public bool IsDigital { get; set; } = false;
    public string? MetaTitle { get; set; } = null;
    public string? MetaDescription { get; set; } = null;
    public List<IFormFile>? ImageFiles { get; set; } = null;
    public List<string>? ImageAltTexts { get; set; } = null;
    public List<int>? ImageDisplayOrders { get; set; } = null;
    public List<bool>? ImageIsPrimary { get; set; } = null;
    public List<CreateProductVariantRequest>? Variants { get; set; } = null;
    public List<CreateProductAttributeValueRequest>? Attributes { get; set; } = null;
    public List<Guid>? TagIds { get; set; } = null;
}
