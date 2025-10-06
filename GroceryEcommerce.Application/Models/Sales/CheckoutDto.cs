namespace GroceryEcommerce.Application.Models.Sales;

public class CheckoutDto
{
    public Guid UserId { get; set; }
    public List<CheckoutItemDto> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CouponCode { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public BillingAddressDto BillingAddress { get; set; } = new();
    public List<PaymentMethodDto> AvailablePaymentMethods { get; set; } = new();
    public List<ShippingOptionDto> AvailableShippingOptions { get; set; } = new();
    public bool IsValid { get; set; }
    public List<string> ValidationErrors { get; set; } = new();
}

public class CheckoutItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string? VariantName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsAvailable { get; set; }
    public string? AvailabilityMessage { get; set; }
}

public class PaymentMethodDto
{
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public decimal? Fee { get; set; }
}

public class ShippingOptionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public int EstimatedDays { get; set; }
    public bool IsActive { get; set; }
}

public class ProcessCheckoutRequest
{
    public Guid UserId { get; set; }
    public short PaymentMethod { get; set; }
    public Guid? ShippingOptionId { get; set; }
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public BillingAddressDto BillingAddress { get; set; } = new();
    public string? CouponCode { get; set; }
    public string? Notes { get; set; }
    public ProcessPaymentRequest? PaymentDetails { get; set; }
}

public class ValidateCheckoutRequest
{
    public Guid UserId { get; set; }
    public List<CheckoutItemDto> Items { get; set; } = new();
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public BillingAddressDto BillingAddress { get; set; } = new();
    public string? CouponCode { get; set; }
}

public class CalculateShippingRequest
{
    public ShippingAddressDto ShippingAddress { get; set; } = new();
    public List<ShippingItemDto> Items { get; set; } = new();
}

public class ShippingItemDto
{
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal Weight { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
}

public class CalculateTaxRequest
{
    public BillingAddressDto BillingAddress { get; set; } = new();
    public List<TaxItemDto> Items { get; set; } = new();
}

public class TaxItemDto
{
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? TaxCategory { get; set; }
}
