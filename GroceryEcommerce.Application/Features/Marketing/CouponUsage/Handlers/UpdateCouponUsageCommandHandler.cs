using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.CouponUsage.Handlers;

public class UpdateCouponUsageCommandHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<UpdateCouponUsageCommandHandler> logger
) : IRequestHandler<UpdateCouponUsageCommand, Result<CouponUsageDto>>
{
    public async Task<Result<CouponUsageDto>> Handle(UpdateCouponUsageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating CouponUsage: {UsageId}", request.UsageId);

        var existingResult = await repository.GetByIdAsync(request.UsageId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            return Result<CouponUsageDto>.Failure("CouponUsage not found.");
        }

        var usage = existingResult.Data;
        usage.DiscountAmount = request.DiscountAmount;

        var updateResult = await repository.UpdateAsync(usage, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update CouponUsage: {UsageId}", request.UsageId);
            return Result<CouponUsageDto>.Failure(updateResult.ErrorMessage ?? "Failed to update CouponUsage.");
        }

        var updatedResult = await repository.GetByIdAsync(request.UsageId, cancellationToken);
        if (!updatedResult.IsSuccess || updatedResult.Data == null)
        {
            return Result<CouponUsageDto>.Failure("Failed to retrieve updated CouponUsage.");
        }

        var dto = mapper.Map<CouponUsageDto>(updatedResult.Data);
        logger.LogInformation("CouponUsage updated: {UsageId}", request.UsageId);
        return Result<CouponUsageDto>.Success(dto);
    }
}

