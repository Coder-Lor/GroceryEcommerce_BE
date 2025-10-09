namespace GroceryEcommerce.Infrastructure.Settings;

public class EmailSettings
{
    public required string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public required string SmtpUser { get; set; }
    public required string SmtpPassword { get; set; }
    public required string SenderEmail { get; set; }
    public required  string SenderName { get; set; }
    public bool EnableSsl { get; set; }
}
