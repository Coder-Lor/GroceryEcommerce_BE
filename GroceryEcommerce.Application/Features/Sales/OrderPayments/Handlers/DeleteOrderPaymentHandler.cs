using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderPayments.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderPayments.Handlers;

public class DeleteOrderPaymentHandler(
    IOrderPaymentRepository repository,
    ILogger<DeleteOrderPaymentHandler> logger
) : IRequestHandler<DeleteOrderPaymentCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order payment: {PaymentId}", request.PaymentId);

            var result = await repository.DeleteAsync(request.PaymentId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order payment: {PaymentId}", request.PaymentId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order payment.");
            }

            logger.LogInformation("Order payment deleted successfully: {PaymentId}", request.PaymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order payment: {PaymentId}", request.PaymentId);
            return Result<bool>.Failure("An error occurred while deleting order payment.");
        }
    }
}

