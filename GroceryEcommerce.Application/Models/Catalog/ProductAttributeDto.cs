using GroceryEcommerce.Application.Common;

namespace GroceryEcommerce.Application.Models.Catalog;

public record ProductAttributeDto
{
    public Guid ProductAttributeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = string.Empty; // Text, Number, Boolean, Date, etc.
    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public record ProductAttributeValueDto
{
    public Guid ProductAttributeValueId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public Guid ProductAttributeId { get; set; }
    public string ProductAttributeName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

// product attribute value command request
public record CreateProductAttributeValueRequest
{
    public Guid ProductId { get; set; }
    public Guid ProductAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;
}

// product attribute value command response
public record CreateProductAttributeValueResponse : ProductAttributeValueDto;

// update product attribute value command response
public record UpdateProductAttributeValueResponse : ProductAttributeValueDto;

public record UpdateProductAttributeValueRequest
{
    public string Value { get; set; } = string.Empty;
}

// product attribute command request
public record CreateProductAttributeRequest : ProductAttributeDto;
public record UpdateProductAttributeRequest : ProductAttributeDto;

// product attribute command response
public record CreateProductAttributeResponse : ProductAttributeDto;
public record UpdateProductAttributeResponse : ProductAttributeDto;

// product attribute value query response