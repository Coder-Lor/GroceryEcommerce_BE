using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderRefunds.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderRefunds.Handlers;

public class ProcessRefundHandler(
    IOrderRefundRepository repository,
    ILogger<ProcessRefundHandler> logger
) : IRequestHandler<ProcessRefundCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ProcessRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing refund: {RefundId}, ProcessedBy: {ProcessedBy}", request.RefundId, request.ProcessedBy);

            var result = await repository.ProcessRefundAsync(request.RefundId, request.ProcessedBy, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to process refund: {RefundId}", request.RefundId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to process refund.");
            }

            logger.LogInformation("Refund processed successfully: {RefundId}", request.RefundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing refund: {RefundId}", request.RefundId);
            return Result<bool>.Failure("An error occurred while processing refund.");
        }
    }
}

