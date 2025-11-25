using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class GetCouponUsagesPagingQueryHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<GetCouponUsagesPagingQueryHandler> logger
) : IRequestHandler<GetCouponUsagesPagingQuery, Result<PagedResult<CouponUsageDto>>>
{
    public async Task<Result<PagedResult<CouponUsageDto>>> Handle(GetCouponUsagesPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting CouponUsages paging - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get CouponUsages paging");
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

