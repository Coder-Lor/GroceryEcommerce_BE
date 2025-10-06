namespace GroceryEcommerce.Application.Models.System;

public class EmailTemplateDto
{
    public Guid EmailTemplateId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? HtmlBody { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemTemplate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? UpdatedByName { get; set; }
    public List<string> AvailableVariables { get; set; } = new();
}

public class CreateEmailTemplateRequest
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? HtmlBody { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSystemTemplate { get; set; } = false;
}

public class UpdateEmailTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? HtmlBody { get; set; }
    public bool IsActive { get; set; }
}

public class EmailTemplateCategoryDto
{
    public string Category { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TemplateCount { get; set; }
    public List<EmailTemplateDto> Templates { get; set; } = new();
}

public class SendEmailRequest
{
    public string TemplateKey { get; set; } = string.Empty;
    public string ToEmail { get; set; } = string.Empty;
    public string? ToName { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new();
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public List<string>? CcEmails { get; set; }
    public List<string>? BccEmails { get; set; }
    public List<EmailAttachmentDto>? Attachments { get; set; }
}

public class EmailAttachmentDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
}
