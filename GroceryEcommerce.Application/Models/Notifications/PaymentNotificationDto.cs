namespace GroceryEcommerce.Application.Models.Notifications;

public class PaymentNotificationDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "VND";
    public string Status { get; set; } = string.Empty; // "Success", "Failed", "Pending"
    public string Message { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public string Type { get; set; } = "PaymentConfirmation"; // Loại thông báo
}

