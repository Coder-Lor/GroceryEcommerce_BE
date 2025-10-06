namespace GroceryEcommerce.Application.Models.Catalog;

public class ProductAttributeDto
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

public class ProductAttributeValueDto
{
    public Guid ProductAttributeValueId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public Guid ProductAttributeId { get; set; }
    public string ProductAttributeName { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateProductAttributeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateProductAttributeRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsFilterable { get; set; }
    public int DisplayOrder { get; set; }
}

public class CreateProductAttributeValueRequest
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public Guid ProductAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;
}

public class UpdateProductAttributeValueRequest
{
    public string Value { get; set; } = string.Empty;
}
