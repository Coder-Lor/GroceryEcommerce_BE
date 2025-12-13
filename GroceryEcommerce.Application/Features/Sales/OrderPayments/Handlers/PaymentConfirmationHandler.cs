using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Notifications;
using GroceryEcommerce.Application.Models.Sales;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class PaymentConfirmationHandler (
    IOrderPaymentRepository orderPaymentRepository,
    ISalesRepository salesRepository,
    INotificationService notificationService,
    ILogger<PaymentConfirmationHandler> logger
) : IRequestHandler<PaymentConfirmationCommand, SepayResponse>
{
    private static readonly Regex TransactionCodeRegex = new(@"[A-Za-z]{2}\d{12}", RegexOptions.Compiled);

    public async Task<SepayResponse> Handle(PaymentConfirmationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var paymentRequest = request.Request;
            logger.LogInformation("Processing payment confirmation for transaction ID: {TransactionId}", paymentRequest.Id);

            var transactionId = DetermineTransactionId(paymentRequest);
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                logger.LogWarning("Unable to determine transaction ID from payment confirmation payload: {@Payload}", paymentRequest);
                return new SepayResponse
                {
                    success = false,
                    message = "Không xác định được mã giao dịch từ payload."
                };
            }

            var existingPaymentResult = await orderPaymentRepository.GetByTransactionIdAsync(transactionId, cancellationToken);
            if (!existingPaymentResult.IsSuccess || existingPaymentResult.Data is null)
            {
                logger.LogWarning("No existing order payment found for transaction ID: {TransactionId}", transactionId);
                return new SepayResponse
                {
                    success = false,
                    message = $"Không tìm thấy giao dịch với mã {transactionId}."
                };
            }

            var orderPayment = existingPaymentResult.Data;
            orderPayment.Status = 2; // Completed
            orderPayment.PaidAt = paymentRequest.TransactionDate ?? DateTime.UtcNow;
            orderPayment.Amount = paymentRequest.TransferAmount;
            orderPayment.GatewayResponse = JsonSerializer.Serialize(paymentRequest);
            orderPayment.UpdatedAt = DateTime.UtcNow;

            var updatePaymentResult = await orderPaymentRepository.UpdateAsync(orderPayment, cancellationToken);
            if (!updatePaymentResult.IsSuccess)
            {
                logger.LogError("Failed to update order payment status for payment {PaymentId}", orderPayment.PaymentId);
                return new SepayResponse
                {
                    success = false,
                    message = updatePaymentResult.ErrorMessage ?? "Không thể cập nhật trạng thái thanh toán."
                };
            }

            var orderResult = await UpdateOrderStatusAsync(orderPayment.OrderId, cancellationToken);
            
            // Gửi thông báo real-time cho người dùng khi thanh toán thành công
            if (orderResult != null && orderResult.UserId != Guid.Empty)
            {
                try
                {
                    var notification = new PaymentNotificationDto
                    {
                        OrderId = orderResult.OrderId,
                        OrderNumber = orderResult.OrderNumber,
                        PaymentId = orderPayment.PaymentId,
                        TransactionId = transactionId,
                        Amount = orderPayment.Amount,
                        Currency = orderPayment.Currency,
                        Status = "Success",
                        Message = $"Thanh toán thành công cho đơn hàng {orderResult.OrderNumber}",
                        PaidAt = orderPayment.PaidAt ?? DateTime.UtcNow,
                        Type = "PaymentConfirmation"
                    };

                    await notificationService.NotifyPaymentSuccessAsync(orderResult.UserId, notification, cancellationToken);
                    logger.LogInformation("Real-time notification sent to user {UserId} for successful payment", orderResult.UserId);
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không fail payment confirmation
                    logger.LogError(ex, "Failed to send real-time notification for payment success to user {UserId}", orderResult.UserId);
                }
            }

            logger.LogInformation("Payment confirmation processed successfully for transaction ID: {TransactionId}", transactionId);
            return new SepayResponse
            {
                success = true,
                message = "Payment confirmation processed successfully."
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment confirmation");
            return new SepayResponse
            {
                success = false,
                message = "An error occurred while processing payment confirmation."
            };
        }
    }

    private static string? DetermineTransactionId(PaymentConfirmationRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            return request.Code.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Content))
        {
            var match = TransactionCodeRegex.Match(request.Content);
            if (match.Success)
            {
                return match.Value;
            }
        }

        return request.Id > 0 ? request.Id.ToString() : null;
    }

    private async Task<GroceryEcommerce.Domain.Entities.Sales.Order?> UpdateOrderStatusAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var orderResult = await salesRepository.GetOrderByIdAsync(orderId, cancellationToken);
        if (!orderResult.IsSuccess || orderResult.Data is null)
        {
            logger.LogWarning("Order not found when updating status for orderId: {OrderId}", orderId);
            return null;
        }

        var order = orderResult.Data;
        
        // Cập nhật PaymentStatus = 2 (Paid) sử dụng method chuyên biệt
        var updatePaymentStatusResult = await salesRepository.UpdateOrderPaymentStatusAsync(orderId, 2, cancellationToken);
        if (!updatePaymentStatusResult.IsSuccess)
        {
            logger.LogError("Failed to update order payment status for orderId: {OrderId}. Error: {Error}", 
                orderId, updatePaymentStatusResult.ErrorMessage);
            return null;
        }

        // Nếu order đang ở trạng thái Pending (1), cập nhật thành Processing (2)
        if (order.Status == 1)
        {
            var updateStatusResult = await salesRepository.UpdateOrderStatusAsync(orderId, null, 2, cancellationToken);
            if (!updateStatusResult.IsSuccess)
            {
                logger.LogWarning("Failed to update order status to Processing for orderId: {OrderId}. Error: {Error}", 
                    orderId, updateStatusResult.ErrorMessage);
                // Không return null vì PaymentStatus đã được cập nhật thành công
            }
        }

        // Lấy lại order đã được cập nhật để return
        var updatedOrderResult = await salesRepository.GetOrderByIdAsync(orderId, cancellationToken);
        if (updatedOrderResult.IsSuccess && updatedOrderResult.Data != null)
        {
            logger.LogInformation("Order status updated successfully for orderId: {OrderId}, PaymentStatus: {PaymentStatus}, Status: {Status}", 
                orderId, updatedOrderResult.Data.PaymentStatus, updatedOrderResult.Data.Status);
            return updatedOrderResult.Data;
        }

        return order;
    }

}
