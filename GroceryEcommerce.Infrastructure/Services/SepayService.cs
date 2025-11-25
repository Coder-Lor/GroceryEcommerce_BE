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

    public async Task<Result<SepayPaymentStatusResponse>> GetPaymentStatusAsync(string transactionId)
    {
        try
        {
            _logger.LogInformation("Getting Sepay payment status for transaction: {TransactionId}", transactionId);

            // Nếu không bật xác thực, trả về mock payment status
            if (!_isAuthenticationEnabled)
            {
                _logger.LogWarning("Sepay authentication is not enabled. Returning mock payment status for transaction: {TransactionId}", transactionId);
                
                return Result<SepayPaymentStatusResponse>.Success(new SepayPaymentStatusResponse
                {
                    Success = true,
                    Message = "Mock payment status (authentication disabled)",
                    Status = "pending",
                    TransactionId = transactionId,
                    Amount = null,
                    PaidAt = null
                });
            }

            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            }
            
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Tạo request với timestamp và signature nếu cần
            var requestData = new
            {
                transaction_id = transactionId,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            string? signature = null;
            if (!string.IsNullOrEmpty(_apiSecret))
            {
                signature = GenerateSignature(requestData, _apiSecret);
            }

            // Gọi API Sepay
            var url = $"/api/v1/payments/{transactionId}/status";
            if (!string.IsNullOrEmpty(signature))
            {
                url += $"?signature={signature}";
            }

            var response = await _httpClient.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sepay API error getting payment status: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return Result<SepayPaymentStatusResponse>.Failure($"Sepay API error: {response.StatusCode}");
            }

            // Parse response
            var result = JsonSerializer.Deserialize<SepayPaymentStatusApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true && result.Data != null)
            {
                _logger.LogInformation("Sepay payment status retrieved successfully. TransactionId: {TransactionId}, Status: {Status}", 
                    transactionId, result.Data.Status);
                return Result<SepayPaymentStatusResponse>.Success(new SepayPaymentStatusResponse
                {
                    Success = true,
                    Message = result.Message ?? "Payment status retrieved successfully",
                    Status = result.Data.Status,
                    TransactionId = result.Data.TransactionId,
                    Amount = result.Data.Amount,
                    PaidAt = result.Data.PaidAt
                });
            }

            return Result<SepayPaymentStatusResponse>.Failure(result?.Message ?? "Failed to get payment status");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Sepay payment status for transaction: {TransactionId}", transactionId);
            return Result<SepayPaymentStatusResponse>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<SepayPaymentResponse>> UpdatePaymentAsync(string transactionId, UpdateSepayPaymentRequest request)
    {
        try
        {
            _logger.LogInformation("Updating Sepay payment for transaction: {TransactionId}", transactionId);

            // Nếu không bật xác thực, trả về mock payment response
            if (!_isAuthenticationEnabled)
            {
                _logger.LogWarning("Sepay authentication is not enabled. Returning mock payment response for transaction: {TransactionId}", transactionId);
                
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = "Mock payment updated (authentication disabled)",
                    PaymentUrl = $"{_baseUrl}/payment/{transactionId}",
                    QrCodeUrl = $"{_baseUrl}/qr/{transactionId}",
                    TransactionId = transactionId,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                });
            }

            // Tạo request body
            var updateRequest = new
            {
                description = request.Description,
                customer_name = request.CustomerName,
                customer_email = request.CustomerEmail,
                customer_phone = request.CustomerPhone,
                return_url = request.ReturnUrl,
                cancel_url = request.CancelUrl,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Loại bỏ các field null
            var requestDict = new Dictionary<string, object>();
            if (updateRequest.description != null) requestDict["description"] = updateRequest.description;
            if (updateRequest.customer_name != null) requestDict["customer_name"] = updateRequest.customer_name;
            if (updateRequest.customer_email != null) requestDict["customer_email"] = updateRequest.customer_email;
            if (updateRequest.customer_phone != null) requestDict["customer_phone"] = updateRequest.customer_phone;
            if (updateRequest.return_url != null) requestDict["return_url"] = updateRequest.return_url;
            if (updateRequest.cancel_url != null) requestDict["cancel_url"] = updateRequest.cancel_url;
            requestDict["timestamp"] = updateRequest.timestamp;

            // Tạo signature nếu có secret key
            string? signature = null;
            if (!string.IsNullOrEmpty(_apiSecret))
            {
                // Serialize Dictionary trực tiếp (không dùng PropertyNamingPolicy vì keys đã là snake_case)
                var json = JsonSerializer.Serialize(requestDict);
                signature = ComputeHmacSha256(json, _apiSecret);
            }

            // Tạo request body
            object requestBody;
            if (!string.IsNullOrEmpty(signature))
            {
                requestBody = new
                {
                    data = requestDict,
                    signature = signature
                };
            }
            else
            {
                requestBody = requestDict;
            }

            var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            }
            
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Gọi API Sepay
            var response = await _httpClient.PutAsync($"/api/v1/payments/{transactionId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sepay API error updating payment: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return Result<SepayPaymentResponse>.Failure($"Sepay API error: {response.StatusCode}");
            }

            // Parse response
            var result = JsonSerializer.Deserialize<SepayApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true && result.Data != null)
            {
                _logger.LogInformation("Sepay payment updated successfully. TransactionId: {TransactionId}", transactionId);
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = "Payment updated successfully",
                    PaymentUrl = result.Data.PaymentUrl,
                    QrCodeUrl = result.Data.QrCodeUrl,
                    TransactionId = result.Data.TransactionId,
                    ExpiresAt = result.Data.ExpiresAt
                });
            }

            return Result<SepayPaymentResponse>.Failure(result?.Message ?? "Failed to update payment");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating Sepay payment for transaction: {TransactionId}", transactionId);
            return Result<SepayPaymentResponse>.Failure($"Error: {ex.Message}");
        }
    }

    public async Task<Result<SepayPaymentResponse>> DeletePaymentAsync(string transactionId)
    {
        try
        {
            _logger.LogInformation("Deleting Sepay payment for transaction: {TransactionId}", transactionId);

            // Nếu không bật xác thực, trả về mock payment response
            if (!_isAuthenticationEnabled)
            {
                _logger.LogWarning("Sepay authentication is not enabled. Returning mock payment response for transaction: {TransactionId}", transactionId);
                
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = "Mock payment deleted (authentication disabled)",
                    TransactionId = transactionId
                });
            }

            // Tạo request với timestamp và signature nếu cần
            var requestData = new
            {
                transaction_id = transactionId,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            string? signature = null;
            if (!string.IsNullOrEmpty(_apiSecret))
            {
                signature = GenerateSignature(requestData, _apiSecret);
            }

            // Cấu hình HttpClient
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();
            
            if (!string.IsNullOrEmpty(_apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            }
            
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Gọi API Sepay
            var url = $"/api/v1/payments/{transactionId}";
            if (!string.IsNullOrEmpty(signature))
            {
                url += $"?signature={signature}";
            }

            var response = await _httpClient.DeleteAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Sepay API error deleting payment: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return Result<SepayPaymentResponse>.Failure($"Sepay API error: {response.StatusCode}");
            }

            // Parse response
            var result = JsonSerializer.Deserialize<SepayApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result?.Success == true)
            {
                _logger.LogInformation("Sepay payment deleted successfully. TransactionId: {TransactionId}", transactionId);
                return Result<SepayPaymentResponse>.Success(new SepayPaymentResponse
                {
                    Success = true,
                    Message = result.Message ?? "Payment deleted successfully",
                    TransactionId = transactionId
                });
            }

            return Result<SepayPaymentResponse>.Failure(result?.Message ?? "Failed to delete payment");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting Sepay payment for transaction: {TransactionId}", transactionId);
            return Result<SepayPaymentResponse>.Failure($"Error: {ex.Message}");
        }
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

    private class SepayPaymentStatusApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public SepayPaymentStatusData? Data { get; set; }
    }

    private class SepayPaymentStatusData
    {
        public string? TransactionId { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}