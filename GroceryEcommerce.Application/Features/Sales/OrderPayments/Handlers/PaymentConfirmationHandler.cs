using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using Microsoft.Extensions.Logging;
using MediatR;


namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class PaymentConfirmationHandler (
    IOrderPaymentRepository repository,
    ILogger<CreateOrderPaymentHandler> logger
) : IRequestHandler<PaymentConfirmationCommand, SepayResponse>
{
    public async Task<SepayResponse> Handle(PaymentConfirmationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing payment confirmation for transaction ID: {TransactionId}", request.Request.Id);
            // Map the incoming request to the OrderPayment entity
            var orderPayment = new OrderPayment
            {
                PaymentId = Guid.NewGuid(),
                //OrderId = request.Request.OrderId,
                PaymentMethod = 3, // Assuming 0 represents SePay
                Amount = request.Request.TransferAmount,
                TransactionId = request.Request.ReferenceCode,
                GatewayResponse = request.Request.Gateway,
                Status = 1, // Assuming 1 represents 'Completed'
                PaidAt = request.Request.TransactionDate,
                CreatedAt = DateTime.UtcNow
            };
            var createResult = await repository.CreateAsync(orderPayment, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order payment from payment confirmation");
                return new SepayResponse
                {
                    success = false,
                    message = createResult.ErrorMessage ?? "Failed to process payment confirmation."
                };
            }
            logger.LogInformation("Payment confirmation processed successfully for transaction ID: {TransactionId}", request.Request.Id);
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
}
