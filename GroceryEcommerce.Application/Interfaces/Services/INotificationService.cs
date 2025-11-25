using GroceryEcommerce.Application.Models.Notifications;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface INotificationService
{
    Task NotifyPaymentSuccessAsync(Guid userId, PaymentNotificationDto notification, CancellationToken cancellationToken = default);
    Task NotifyPaymentFailedAsync(Guid userId, PaymentNotificationDto notification, CancellationToken cancellationToken = default);
}

