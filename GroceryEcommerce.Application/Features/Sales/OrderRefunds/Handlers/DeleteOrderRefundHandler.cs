using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class DeleteOrderRefundHandler(
    IOrderRefundRepository repository,
    ILogger<DeleteOrderRefundHandler> logger
) : IRequestHandler<DeleteOrderRefundCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order refund: {RefundId}", request.RefundId);

            var result = await repository.DeleteAsync(request.RefundId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order refund: {RefundId}", request.RefundId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order refund.");
            }

            logger.LogInformation("Order refund deleted successfully: {RefundId}", request.RefundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order refund: {RefundId}", request.RefundId);
            return Result<bool>.Failure("An error occurred while deleting order refund.");
        }
    }
}

