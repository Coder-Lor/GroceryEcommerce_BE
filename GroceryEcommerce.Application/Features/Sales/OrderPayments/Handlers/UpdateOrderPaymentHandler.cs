using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class UpdateOrderPaymentHandler(
    IMapper mapper,
    IOrderPaymentRepository repository,
    ILogger<UpdateOrderPaymentHandler> logger
) : IRequestHandler<UpdateOrderPaymentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order payment: {PaymentId}", request.PaymentId);

            var paymentResult = await repository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (!paymentResult.IsSuccess || paymentResult.Data is null)
            {
                return Result<bool>.Failure("Order payment not found.");
            }

            var payment = paymentResult.Data;
            payment.PaymentMethod = request.Request.PaymentMethod;
            payment.Amount = request.Request.Amount;
            payment.TransactionId = request.Request.TransactionId;
            payment.Status = request.Request.Status;
            payment.UpdatedAt = DateTime.UtcNow;

            var updateResult = await repository.UpdateAsync(payment, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order payment: {PaymentId}", request.PaymentId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order payment.");
            }

            logger.LogInformation("Order payment updated successfully: {PaymentId}", request.PaymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order payment: {PaymentId}", request.PaymentId);
            return Result<bool>.Failure("An error occurred while updating order payment.");
        }
    }
}

