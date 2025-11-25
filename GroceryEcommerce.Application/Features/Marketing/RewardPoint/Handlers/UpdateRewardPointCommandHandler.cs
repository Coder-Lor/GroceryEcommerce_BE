using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class UpdateRewardPointCommandHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<UpdateRewardPointCommandHandler> logger
) : IRequestHandler<UpdateRewardPointCommand, Result<RewardPointDto>>
{
    public async Task<Result<RewardPointDto>> Handle(UpdateRewardPointCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating RewardPoint: {RewardPointId}", request.RewardPointId);

        var existingResult = await repository.GetByIdAsync(request.RewardPointId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            return Result<RewardPointDto>.Failure("RewardPoint not found.");
        }

        var rewardPoint = existingResult.Data;
        rewardPoint.Points = request.Points;
        rewardPoint.Reason = request.Reason;
        rewardPoint.ExpiresAt = request.ExpiresAt;

        var updateResult = await repository.UpdateAsync(rewardPoint, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update RewardPoint: {RewardPointId}", request.RewardPointId);
            return Result<RewardPointDto>.Failure(updateResult.ErrorMessage ?? "Failed to update RewardPoint.");
        }

        var updatedResult = await repository.GetByIdAsync(request.RewardPointId, cancellationToken);
        if (!updatedResult.IsSuccess || updatedResult.Data == null)
        {
            return Result<RewardPointDto>.Failure("Failed to retrieve updated RewardPoint.");
        }

        var dto = mapper.Map<RewardPointDto>(updatedResult.Data);
        logger.LogInformation("RewardPoint updated: {RewardPointId}", request.RewardPointId);
        return Result<RewardPointDto>.Success(dto);
    }
}

