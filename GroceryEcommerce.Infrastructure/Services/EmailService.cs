using GroceryEcommerce.Application.Interfaces.Services;

namespace GroceryEcommerce.Infrastructure.Services;

public class EmailService: IEmailService
{
    public Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendWelcomeEmailAsync(string to, string userName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal totalAmount)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendPasswordResetEmailAsync(string to, string resetToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendEmailVerificationAsync(string to, string verificationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendOrderStatusUpdateEmailAsync(string to, string orderNumber, string status)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendPromotionalEmailAsync(string to, string promotionTitle, string promotionContent)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}