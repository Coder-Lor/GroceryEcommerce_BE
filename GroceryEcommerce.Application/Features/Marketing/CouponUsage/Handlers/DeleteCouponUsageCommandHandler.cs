using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class DeleteCouponUsageCommandHandler(
    ICouponUsageRepository repository,
    ILogger<DeleteCouponUsageCommandHandler> logger
) : IRequestHandler<DeleteCouponUsageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCouponUsageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting CouponUsage: {UsageId}", request.UsageId);

        var result = await repository.DeleteAsync(request.UsageId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete CouponUsage: {UsageId}", request.UsageId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete CouponUsage.");
        }

        logger.LogInformation("CouponUsage deleted: {UsageId}", request.UsageId);
        return Result<bool>.Success(true);
    }
}

