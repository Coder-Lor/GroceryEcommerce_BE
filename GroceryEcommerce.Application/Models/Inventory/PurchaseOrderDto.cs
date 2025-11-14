namespace GroceryEcommerce.Application.Models.Inventory;

public class PurchaseOrderDto
{
    public Guid PurchaseOrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedByName { get; set; } = string.Empty;
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
}

public class PurchaseOrderItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; }
    public int ReceivedQuantity { get; set; }
    public decimal TotalCost { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePurchaseOrderRequest
{
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public List<CreatePurchaseOrderItemRequest> Items { get; set; } = new();
}

public class UpdatePurchaseOrderRequest
{
    public DateTime? ExpectedDeliveryDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short Status { get; set; }
    public string? Notes { get; set; }
}

public class CreatePurchaseOrderItemRequest
{
    public Guid ProductId { get; set; }
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; }
}

public class UpdatePurchaseOrderItemRequest
{
    public decimal UnitCost { get; set; }
    public int Quantity { get; set; }
    public int ReceivedQuantity { get; set; }
}
