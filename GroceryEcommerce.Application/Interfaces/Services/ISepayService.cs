using GroceryEcommerce.Application.Common;

namespace GroceryEcommerce.Application.Interfaces.Services;


public class CreateSepayPaymentRequest
{
    public Guid OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class SepayPaymentResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? PaymentUrl { get; set; }
    public string? QrCodeUrl { get; set; }
    public string? TransactionId { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class SepayPaymentStatusResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; } 
    public string? TransactionId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class UpdateSepayPaymentRequest
{
    public string? Description { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerPhone { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public interface ISepayService
{
    Task<Result<SepayPaymentResponse>> CreatePaymentAsync(CreateSepayPaymentRequest request);
    Task<Result<SepayPaymentStatusResponse>> GetPaymentStatusAsync(string transactionId);
    Task<Result<SepayPaymentResponse>> UpdatePaymentAsync(string transactionId, UpdateSepayPaymentRequest request);
    Task<Result<SepayPaymentResponse>> DeletePaymentAsync(string transactionId);
}