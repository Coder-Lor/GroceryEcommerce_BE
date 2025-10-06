namespace GroceryEcommerce.Application.Models.Sales;

public class OrderPaymentDto
{
    public Guid OrderPaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public short PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public string PaymentStatusName { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedByName { get; set; }
}

public class CreateOrderPaymentRequest
{
    public Guid OrderId { get; set; }
    public short PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public short Status { get; set; }
    public string? Notes { get; set; }
}

public class UpdateOrderPaymentRequest
{
    public short PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public short Status { get; set; }
    public string? Notes { get; set; }
}

public class ProcessPaymentRequest
{
    public short PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentGateway { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpiryDate { get; set; }
    public string? Cvv { get; set; }
    public string? CardHolderName { get; set; }
    public string? BillingAddress { get; set; }
}
