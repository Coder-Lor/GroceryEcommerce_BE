using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class GetCouponUsagesByUserIdQueryHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<GetCouponUsagesByUserIdQueryHandler> logger
) : IRequestHandler<GetCouponUsagesByUserIdQuery, Result<PagedResult<CouponUsageDto>>>
{
    public async Task<Result<PagedResult<CouponUsageDto>>> Handle(GetCouponUsagesByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting CouponUsages by UserId: {UserId}", request.UserId);

        var result = await repository.GetByUserIdAsync(request.UserId, request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get CouponUsages by UserId: {UserId}", request.UserId);
            return Result<PagedResult<CouponUsageDto>>.Failure(result.ErrorMessage ?? "Failed to get CouponUsages.");
        }

        var dtos = mapper.Map<List<CouponUsageDto>>(result.Data?.Items ?? new List<Domain.Entities.Marketing.CouponUsage>());
        var pagedResult = new PagedResult<CouponUsageDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<CouponUsageDto>>.Success(pagedResult);
    }
}

