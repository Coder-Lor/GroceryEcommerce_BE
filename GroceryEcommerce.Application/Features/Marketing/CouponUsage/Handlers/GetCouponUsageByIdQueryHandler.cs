using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class GetCouponUsageByIdQueryHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<GetCouponUsageByIdQueryHandler> logger
) : IRequestHandler<GetCouponUsageByIdQuery, Result<CouponUsageDto?>>
{
    public async Task<Result<CouponUsageDto?>> Handle(GetCouponUsageByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting CouponUsage by Id: {UsageId}", request.UsageId);

        var result = await repository.GetByIdAsync(request.UsageId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get CouponUsage: {UsageId}", request.UsageId);
            return Result<CouponUsageDto?>.Failure(result.ErrorMessage ?? "Failed to get CouponUsage.");
        }

        if (result.Data == null)
        {
            return Result<CouponUsageDto?>.Success(null);
        }

        var dto = mapper.Map<CouponUsageDto>(result.Data);
        return Result<CouponUsageDto?>.Success(dto);
    }
}

