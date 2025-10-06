namespace GroceryEcommerce.Application.Models.System;

public class SystemSettingDto
{
    public Guid SystemSettingId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
}

public class CreateSystemSettingRequest
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public bool IsPublic { get; set; }
}

public class UpdateSystemSettingRequest
{
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public bool IsPublic { get; set; }
}

public class SystemSettingCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SettingCount { get; set; }
    public List<SystemSettingDto> Settings { get; set; } = new();
}
