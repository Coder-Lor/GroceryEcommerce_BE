using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class UpdatePaymentStatusHandler(
    IOrderPaymentRepository repository,
    ILogger<UpdatePaymentStatusHandler> logger
) : IRequestHandler<UpdatePaymentStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating payment status: {PaymentId}, Status: {Status}", request.PaymentId, request.Status);

            var result = await repository.UpdatePaymentStatusAsync(request.PaymentId, request.Status, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update payment status: {PaymentId}", request.PaymentId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update payment status.");
            }

            logger.LogInformation("Payment status updated successfully: {PaymentId}", request.PaymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating payment status: {PaymentId}", request.PaymentId);
            return Result<bool>.Failure("An error occurred while updating payment status.");
        }
    }
}

