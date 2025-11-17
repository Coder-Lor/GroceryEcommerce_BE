using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class UpdateOrderRefundHandler(
    IMapper mapper,
    IOrderRefundRepository repository,
    ILogger<UpdateOrderRefundHandler> logger
) : IRequestHandler<UpdateOrderRefundCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order refund: {RefundId}", request.RefundId);

            var refundResult = await repository.GetByIdAsync(request.RefundId, cancellationToken);
            if (!refundResult.IsSuccess || refundResult.Data is null)
            {
                return Result<bool>.Failure("Order refund not found.");
            }

            var refund = refundResult.Data;
            refund.Amount = request.Request.Amount;
            refund.Reason = request.Request.Reason;
            refund.Status = request.Request.Status;

            var updateResult = await repository.UpdateAsync(refund, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order refund: {RefundId}", request.RefundId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order refund.");
            }

            logger.LogInformation("Order refund updated successfully: {RefundId}", request.RefundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order refund: {RefundId}", request.RefundId);
            return Result<bool>.Failure("An error occurred while updating order refund.");
        }
    }
}

