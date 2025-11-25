using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.RewardPoint.Handlers;

public class GetRewardPointsPagingQueryHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<GetRewardPointsPagingQueryHandler> logger
) : IRequestHandler<GetRewardPointsPagingQuery, Result<PagedResult<RewardPointDto>>>
{
    public async Task<Result<PagedResult<RewardPointDto>>> Handle(GetRewardPointsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting RewardPoints paging - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get RewardPoints paging");
            return Result<PagedResult<RewardPointDto>>.Failure(result.ErrorMessage ?? "Failed to get RewardPoints.");
        }

        var dtos = mapper.Map<List<RewardPointDto>>(result.Data?.Items ?? new List<Domain.Entities.Marketing.RewardPoint>());
        var pagedResult = new PagedResult<RewardPointDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<RewardPointDto>>.Success(pagedResult);
    }
}

