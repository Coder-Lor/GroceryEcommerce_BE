using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class DeleteRewardPointCommandHandler(
    IRewardPointRepository repository,
    ILogger<DeleteRewardPointCommandHandler> logger
) : IRequestHandler<DeleteRewardPointCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteRewardPointCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting RewardPoint: {RewardPointId}", request.RewardPointId);

        var result = await repository.DeleteAsync(request.RewardPointId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete RewardPoint: {RewardPointId}", request.RewardPointId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete RewardPoint.");
        }

        logger.LogInformation("RewardPoint deleted: {RewardPointId}", request.RewardPointId);
        return Result<bool>.Success(true);
    }
}

