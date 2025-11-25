using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class GetRewardPointByIdQueryHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<GetRewardPointByIdQueryHandler> logger
) : IRequestHandler<GetRewardPointByIdQuery, Result<RewardPointDto?>>
{
    public async Task<Result<RewardPointDto?>> Handle(GetRewardPointByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting RewardPoint by Id: {RewardPointId}", request.RewardPointId);

        var result = await repository.GetByIdAsync(request.RewardPointId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get RewardPoint: {RewardPointId}", request.RewardPointId);
            return Result<RewardPointDto?>.Failure(result.ErrorMessage ?? "Failed to get RewardPoint.");
        }

        if (result.Data == null)
        {
            return Result<RewardPointDto?>.Success(null);
        }

        var dto = mapper.Map<RewardPointDto>(result.Data);
        return Result<RewardPointDto?>.Success(dto);
    }
}

