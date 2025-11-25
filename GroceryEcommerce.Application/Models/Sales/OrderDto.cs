namespace GroceryEcommerce.Application.Models.Sales;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CreatedByName { get; set; }
    public decimal TotalAmount { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public short PaymentStatus { get; set; }
    public string PaymentStatusName { get; set; } = string.Empty;
    public short PaymentMethod { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required string ShippingFullAddress { get; set; }
    public required string BillingFullAddress { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    // Payment information (for bank transfer)
    public string? PaymentUrl { get; set; }
    public string? QrCodeUrl { get; set; }
    public string? PaymentTransactionId { get; set; }
}

public class OrderDetailDto : OrderDto
{
    public List<OrderStatusHistoryDto> StatusHistory { get; set; }
    public List<OrderPaymentDto> Payments { get; set; }
    public List<OrderShipmentDto> Shipments { get; set; }
    public List<OrderRefundDto> Refunds { get; set; }
}

public class OrderItemDto
{
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderStatusHistoryDto
{
    public Guid OrderStatusHistoryId { get; set; }
    public Guid OrderId { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
}

public class ShippingAddressDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class BillingAddressDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}

public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short PaymentMethod { get; set; }
    public string? CouponCode { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public BillingAddressDto BillingAddress { get; set; } = new();
    public string? Notes { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class UpdateOrderRequest
{
    public short Status { get; set; }
    public short PaymentStatus { get; set; }
    public string? Notes { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
}

public class CreateOrderItemRequest
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderItemRequest
{
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}

public class CreateOrderStatusHistoryRequest
{
    public Guid OrderId { get; set; }
    public short? FromStatus { get; set; }
    public short ToStatus { get; set; }
    public string? Comment { get; set; }
    public Guid CreatedBy { get; set; }
}
