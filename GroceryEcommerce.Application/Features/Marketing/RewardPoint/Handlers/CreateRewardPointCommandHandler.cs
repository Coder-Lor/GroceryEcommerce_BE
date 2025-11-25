using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class CreateRewardPointCommandHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<CreateRewardPointCommandHandler> logger
) : IRequestHandler<CreateRewardPointCommand, Result<RewardPointDto>>
{
    public async Task<Result<RewardPointDto>> Handle(CreateRewardPointCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating RewardPoint for UserId: {UserId}, Points: {Points}", request.UserId, request.Points);

        var rewardPoint = new Domain.Entities.Marketing.RewardPoint
        {
            RewardId = Guid.NewGuid(),
            UserId = request.UserId,
            Points = request.Points,
            Reason = request.Reason,
            ExpiresAt = request.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };

        var result = await repository.CreateAsync(rewardPoint, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create RewardPoint");
            return Result<RewardPointDto>.Failure(result.ErrorMessage ?? "Failed to create RewardPoint.");
        }

        if (result.Data is null)
        {
            logger.LogError("RewardPoint creation returned null entity");
            return Result<RewardPointDto>.Failure("Failed to create RewardPoint.");
        }

        var dto = mapper.Map<RewardPointDto>(result.Data);
        logger.LogInformation("RewardPoint created: {RewardId}", result.Data.RewardId);
        return Result<RewardPointDto>.Success(dto);
    }
}

