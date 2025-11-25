using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Notifications;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using GroceryEcommerce.API.Hubs;

namespace GroceryEcommerce.API.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyPaymentSuccessAsync(Guid userId, PaymentNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var groupName = $"User_{userId}";
            
            _logger.LogInformation("Sending payment success notification to user {UserId} for order {OrderId}", 
                userId, notification.OrderId);
            
            await _hubContext.Clients.Group(groupName).SendAsync("PaymentSuccess", notification, cancellationToken);
            
            _logger.LogInformation("Payment success notification sent successfully to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending payment success notification to user {UserId}", userId);
            // Không throw exception để không ảnh hưởng đến flow thanh toán
        }
    }

    public async Task NotifyPaymentFailedAsync(Guid userId, PaymentNotificationDto notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var groupName = $"User_{userId}";
            
            _logger.LogInformation("Sending payment failed notification to user {UserId} for order {OrderId}", 
                userId, notification.OrderId);
            
            await _hubContext.Clients.Group(groupName).SendAsync("PaymentFailed", notification, cancellationToken);
            
            _logger.LogInformation("Payment failed notification sent successfully to user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending payment failed notification to user {UserId}", userId);
            // Không throw exception để không ảnh hưởng đến flow thanh toán
        }
    }
}

