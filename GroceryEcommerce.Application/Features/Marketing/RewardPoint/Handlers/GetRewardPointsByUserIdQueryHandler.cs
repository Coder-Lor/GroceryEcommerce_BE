using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class GetRewardPointsByUserIdQueryHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<GetRewardPointsByUserIdQueryHandler> logger
) : IRequestHandler<GetRewardPointsByUserIdQuery, Result<List<RewardPointDto>>>
{
    public async Task<Result<List<RewardPointDto>>> Handle(GetRewardPointsByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting RewardPoints by UserId: {UserId}", request.UserId);

        var result = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get RewardPoints by UserId: {UserId}", request.UserId);
            return Result<List<RewardPointDto>>.Failure(result.ErrorMessage ?? "Failed to get RewardPoints.");
        }

        var dtos = mapper.Map<List<RewardPointDto>>(result.Data ?? new List<Domain.Entities.Marketing.RewardPoint>());
        return Result<List<RewardPointDto>>.Success(dtos);
    }
}

