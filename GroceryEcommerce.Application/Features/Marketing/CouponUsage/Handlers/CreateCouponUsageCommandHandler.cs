using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class CreateCouponUsageCommandHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<CreateCouponUsageCommandHandler> logger
) : IRequestHandler<CreateCouponUsageCommand, Result<CouponUsageDto>>
{
    public async Task<Result<CouponUsageDto>> Handle(CreateCouponUsageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating CouponUsage for CouponId: {CouponId}, UserId: {UserId}", request.CouponId, request.UserId);

        var usage = new Domain.Entities.Marketing.CouponUsage
        {
            UsageId = Guid.NewGuid(),
            CouponId = request.CouponId,
            UserId = request.UserId,
            OrderId = request.OrderId,
            DiscountAmount = request.DiscountAmount,
            UsedAt = DateTime.UtcNow
        };

        var result = await repository.CreateAsync(usage, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create CouponUsage");
            return Result<CouponUsageDto>.Failure(result.ErrorMessage ?? "Failed to create CouponUsage.");
        }

        if (result.Data is null)
        {
            logger.LogError("CouponUsage creation returned null entity");
            return Result<CouponUsageDto>.Failure("Failed to create CouponUsage.");
        }

        var dto = mapper.Map<CouponUsageDto>(result.Data);
        logger.LogInformation("CouponUsage created: {UsageId}", result.Data.UsageId);
        return Result<CouponUsageDto>.Success(dto);
    }
}

