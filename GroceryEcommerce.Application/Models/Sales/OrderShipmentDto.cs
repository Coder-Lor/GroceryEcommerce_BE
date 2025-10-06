namespace GroceryEcommerce.Application.Models.Sales;

public class OrderShipmentDto
{
    public Guid OrderShipmentId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid? ShipmentCarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ShippedByName { get; set; }
    public required string ShippingAddress { get; set; }
    public List<ShipmentItemDto> Items { get; set; } = new();
}

public class ShipmentItemDto
{
    public Guid ShipmentItemId { get; set; }
    public Guid OrderShipmentId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ShipmentCarrierDto
{
    public Guid ShipmentCarrierId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateOrderShipmentRequest
{
    public Guid OrderId { get; set; }
    public Guid? ShipmentCarrierId { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateShipmentItemRequest> Items { get; set; } = new();
}

public class UpdateOrderShipmentRequest
{
    public Guid? ShipmentCarrierId { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public short Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateShipmentItemRequest
{
    public Guid OrderItemId { get; set; }
    public int Quantity { get; set; }
}

public class CreateShipmentCarrierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateShipmentCarrierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
}
