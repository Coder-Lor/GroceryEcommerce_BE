using Microsoft.AspNetCore.Http;

namespace GroceryEcommerce.Application.Models.Catalog;

public record ProductImageDto
{
    public Guid ProductImageId { get; set; }
    public Guid ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record CreateProductImageResponse : ProductImageDto;

public record UpdateProductImageResponse : ProductImageDto;

public class CreateProductImageRequest
{
    public Guid ProductId { get; set; }
    public string? ImageUrl { get; set; } // URL từ Azure Blob Storage (nếu đã upload trước đó)
    public IFormFile? ImageFile { get; set; } // File upload trực tiếp
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}

public class UpdateProductImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
