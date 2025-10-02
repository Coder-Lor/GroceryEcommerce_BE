namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false);
    Task<bool> SendWelcomeEmailAsync(string to, string userName);
    Task<bool> SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal totalAmount);
    Task<bool> SendPasswordResetEmailAsync(string to, string resetToken);
    Task<bool> SendEmailVerificationAsync(string to, string verificationToken);
    Task<bool> SendOrderStatusUpdateEmailAsync(string to, string orderNumber, string status);
    Task<bool> SendPromotionalEmailAsync(string to, string promotionTitle, string promotionContent);
    Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken);
}