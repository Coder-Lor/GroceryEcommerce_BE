namespace GroceryEcommerce.Application.Models.Sales;

public class OrderRefundDto
{
    public Guid OrderRefundId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public Guid RequestedBy { get; set; }
    public string? RequestedByName { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public Guid? ProcessedBy { get; set; }
    public string? ProcessedByName { get; set; }
}

public class CreateOrderRefundRequest
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateOrderRefundRequest
{
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public short Status { get; set; }
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
}
