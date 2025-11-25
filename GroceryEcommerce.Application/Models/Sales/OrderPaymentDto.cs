using System.Text.Json.Serialization;
using GroceryEcommerce.Application.Common.Converters;

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

public record PaymentConfirmationRequest
{
    // ID giao dịch trên SePay
    public long Id { get; set; }

    // Brand name của ngân hàng
    public string Gateway { get; set; } = string.Empty;

    // Thời gian xảy ra giao dịch phía ngân hàng (format string / unix timestamp từ SePay)
    [JsonConverter(typeof(FlexibleDateTimeConverter))]
    public DateTime? TransactionDate { get; set; }

    // Số tài khoản ngân hàng
    public string? AccountNumber { get; set; }

    // Mã code thanh toán (có thể null)
    public string? Code { get; set; }

    // Nội dung chuyển khoản
    public string? Content { get; set; }

    // Loại giao dịch: "in" (tiền vào) | "out" (tiền ra)
    public string? TransferType { get; set; }

    // Số tiền giao dịch
    public decimal TransferAmount { get; set; }

    // Số dư tài khoản (lũy kế)
    public decimal Accumulated { get; set; }

    // Tài khoản ngân hàng phụ (nếu có)
    public string? SubAccount { get; set; }

    // Mã tham chiếu của tin nhắn sms
    public string? ReferenceCode { get; set; }

    // Mô tả
    public string? Description { get; set; }
}

public class SepayResponse
{
    public bool success { get; set; }
    public string? message { get; set; }
}
