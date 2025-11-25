using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Infrastructure.Services;

public class SepayService(HttpClient httpClient, IConfiguration configuration, ILogger<SepayService> logger)
    : ISepayService
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<SepayService> _logger = logger;
    private readonly string _baseUrl = configuration["Sepay:BaseUrl"] ?? "https://api.sepay.vn";
    private readonly string? _apiKey = configuration["Sepay:ApiKey"];
    private readonly string? _apiSecret = configuration["Sepay:ApiSecret"];
    private readonly string _webhookUrl = configuration["Sepay:WebhookUrl"] ?? $"{configuration["BaseUrl"] ?? "https://localhost"}/api/order-payment/payment-confirmation";
    private readonly bool _isAuthenticationEnabled = !string.IsNullOrEmpty(configuration["Sepay:ApiKey"]) && !string.IsNullOrEmpty(configuration["Sepay:ApiSecret"]);

    public async Task<Result<SepayPaymentResponse>> CreatePaymentAsync(CreateSepayPaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Creating Sepay payment for order: {OrderId}", request.OrderId);

            // Nếu không bật xác thực, trả về mock payment response
            if (!_isAuthenticationEnabled)
            {
                _logger.LogWarning("Sepay authentication is not enabled. Returning mock payment response for order: {OrderId}", request.OrderId);
                
                // Tạo mock transaction ID
                var mockTransactionId = $"MOCK_{request.OrderId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = "Mock payment request created (authentication disabled)",
                    PaymentUrl = $"{_baseUrl}/payment/{mockTransactionId}",
                    QrCodeUrl = $"{_baseUrl}/qr/{mockTransactionId}",
                    TransactionId = mockTransactionId,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }

            // Tạo request body theo format Sepay API
            var paymentRequest = new
            {
                order_id = request.OrderId.ToString(),
                order_number = request.OrderNumber,
                amount = request.Amount,
                description = request.Description ?? $"Thanh toán đơn hàng {request.OrderNumber}",
                customer_name = request.CustomerName,
                customer_email = request.CustomerEmail,
                customer_phone = request.CustomerPhone,
                return_url = request.ReturnUrl,
                cancel_url = request.CancelUrl,
                webhook_url = _webhookUrl,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Tạo signature nếu có secret key
            string? signature = null;
            if (!string.IsNullOrEmpty(_apiSecret))
            {
                signature = GenerateSignature(paymentRequest, _apiSecret);
            }

            // Tạo request body
            object requestBody;
            if (!string.IsNullOrEmpty(signature))
            {
                requestBody = new
                {
                    data = paymentRequest,
                    signature = signature
                };
            }
            else
            {
                requestBody = paymentRequest;
            }

            var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            
            // Chỉ thêm API key header nếu có
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            }
            
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Gọi API Sepay
            var response = await _httpClient.PostAsync("/api/v1/payments/create", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sepay API error: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return Result<SepayPaymentResponse>.Failure($"Sepay API error: {response.StatusCode}");
            }

            // Parse response
            var result = JsonSerializer.Deserialize<SepayApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true && result.Data != null)
            {
                _logger.LogInformation("Sepay payment created successfully. TransactionId: {TransactionId}", result.Data.TransactionId);
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = "Payment request created successfully",
                    PaymentUrl = result.Data.PaymentUrl,
                    QrCodeUrl = result.Data.QrCodeUrl,
                    TransactionId = result.Data.TransactionId,
                    ExpiresAt = result.Data.ExpiresAt
                });
            }

            return Result<SepayPaymentResponse>.Failure(result?.Message ?? "Failed to create payment request");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating Sepay payment for order: {OrderId}", request.OrderId);
            return Result<SepayPaymentResponse>.Failure($"Error: {ex.Message}");
        }
    }

    public Task<Result<SepayPaymentStatusResponse>> GetPaymentStatusAsync(string transactionId)
    {
        // TODO: Implement khi cần
        throw new NotImplementedException();
    }

    public Task<Result<SepayPaymentResponse>> UpdatePaymentAsync(string transactionId, UpdateSepayPaymentRequest request)
    {
        // TODO: Implement khi cần
        throw new NotImplementedException();
    }

    public Task<Result<SepayPaymentResponse>> DeletePaymentAsync(string transactionId)
    {
        // TODO: Implement khi cần
        throw new NotImplementedException();
    }

    private string GenerateSignature(object data, string secret)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return ComputeHmacSha256(json, secret);
    }

    private string ComputeHmacSha256(string data, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes).ToLower();
    }

    // Helper classes cho API response
    private class SepayApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public SepayPaymentData? Data { get; set; }
    }

    private class SepayPaymentData
    {
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}