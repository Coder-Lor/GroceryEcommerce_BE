using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace GroceryEcommerce.Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = body
            };

            using var client = new SmtpClient();
            
            // Kết nối với server SMTP
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, 
                _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
            
            // Xác thực
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPassword);
            
            // Gửi email
            await client.SendAsync(message);
            
            // Ngắt kết nối
            await client.DisconnectAsync(true);
            
            logger.LogInformation("Email sent successfully to {EmailAddress}", to);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending email to {EmailAddress}", to);
            return false;
        }
    }

    public async Task<bool> SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "Chào mừng bạn đến với Grocery Store!";
        var body = $@"
            <html>
            <body>
                <h2>Xin chào {userName}!</h2>
                <p>Chào mừng bạn đã đăng ký tài khoản tại Grocery Store.</p>
                <p>Chúng tôi rất vui khi có bạn là một phần của cộng đồng mua sắm của chúng tôi.</p>
                <p>Hãy bắt đầu mua sắm ngay hôm nay!</p>
                <br/>
                <p>Trân trọng,<br/>Grocery Store Team</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendOrderConfirmationEmailAsync(string to, string orderNumber, decimal totalAmount)
    {
        var subject = $"Xác nhận đơn hàng #{orderNumber}";
        var body = $@"
            <html>
            <body>
                <h2>Đơn hàng của bạn đã được xác nhận!</h2>
                <p><strong>Mã đơn hàng:</strong> {orderNumber}</p>
                <p><strong>Tổng giá trị:</strong> {totalAmount:C}</p>
                <p>Chúng tôi sẽ bắt đầu xử lý đơn hàng của bạn ngay lập tức.</p>
                <p>Bạn sẽ nhận được email thông báo khi đơn hàng được giao.</p>
                <br/>
                <p>Cảm ơn bạn đã mua sắm tại Grocery Store!</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "Đặt lại mật khẩu";
        var resetUrl = $"https://yourwebsite.com/reset-password?token={resetToken}";
        var body = $@"
            <html>
            <body>
                <h2>Đặt lại mật khẩu</h2>
                <p>Bạn đã yêu cầu đặt lại mật khẩu. Nhấp vào liên kết bên dưới để tiếp tục:</p>
                <p><a href='{resetUrl}'>Đặt lại mật khẩu</a></p>
                <p>Liên kết này sẽ hết hạn sau 1 giờ.</p>
                <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
                <br/>
                <p>Trân trọng,<br/>Grocery Store Team</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendEmailVerificationAsync(string to, string verificationToken)
    {
        var subject = "Xác thực email của bạn";
        var verificationUrl = $"https://yourwebsite.com/verify-email?token={verificationToken}";
        var body = $@"
            <html>
            <body>
                <h2>Xác thực địa chỉ email</h2>
                <p>Vui lòng xác thực địa chỉ email của bạn bằng cách nhấp vào liên kết bên dưới:</p>
                <p><a href='{verificationUrl}'>Xác thực Email</a></p>
                <p>Liên kết này sẽ hết hạn sau 24 giờ.</p>
                <br/>
                <p>Trân trọng,<br/>Grocery Store Team</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendOrderStatusUpdateEmailAsync(string to, string orderNumber, string status)
    {
        var subject = $"Cập nhật trạng thái đơn hàng #{orderNumber}";
        var statusMessage = status.ToLower() switch
        {
            "processing" => "đang được xử lý",
            "shipped" => "đã được vận chuyển",
            "delivered" => "đã được giao thành công",
            "cancelled" => "đã bị hủy",
            _ => status
        };
        
        var body = $@"
            <html>
            <body>
                <h2>Cập nhật đơn hàng</h2>
                <p>Đơn hàng <strong>#{orderNumber}</strong> của bạn {statusMessage}.</p>
                <p>Bạn có thể theo dõi chi tiết đơn hàng trong tài khoản của mình.</p>
                <br/>
                <p>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>
                <p>Trân trọng,<br/>Grocery Store Team</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task<bool> SendPromotionalEmailAsync(string to, string promotionTitle, string promotionContent)
    {
        var subject = $" {promotionTitle}";
        var body = $@"
            <html>
            <body>
                <h1 style='color: #ff6600;'>{promotionTitle}</h1>
                <div>{promotionContent}</div>
                <br/>
                <p><a href='https://yourwebsite.com/promotions' style='background-color: #ff6600; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Xem chi tiết</a></p>
                <br/>
                <p>Đừng bỏ lỡ cơ hội này!</p>
                <p>Trân trọng,<br/>Grocery Store Team</p>
            </body>
            </html>";
        
        return await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken, CancellationToken cancellationToken)
    {
        await SendPasswordResetEmailAsync(email, resetToken);
    }
}