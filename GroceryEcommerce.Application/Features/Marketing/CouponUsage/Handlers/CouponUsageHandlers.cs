using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Commands;
using GroceryEcommerce.Application.Features.Marketing.CouponUsage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using GroceryEcommerce.Domain.Entities.Marketing;
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

        var usage = new CouponUsage
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

        var dto = mapper.Map<CouponUsageDto>(result.Data);
        logger.LogInformation("CouponUsage created: {UsageId}", result.Data.UsageId);
        return Result<CouponUsageDto>.Success(dto);
    }
}

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

public class DeleteCouponUsageCommandHandler(
    ICouponUsageRepository repository,
    ILogger<DeleteCouponUsageCommandHandler> logger
) : IRequestHandler<DeleteCouponUsageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteCouponUsageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting CouponUsage: {UsageId}", request.UsageId);

        var result = await repository.DeleteAsync(request.UsageId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete CouponUsage: {UsageId}", request.UsageId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete CouponUsage.");
        }

        logger.LogInformation("CouponUsage deleted: {UsageId}", request.UsageId);
        return Result<bool>.Success(true);
    }
}

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

        var dtos = mapper.Map<List<CouponUsageDto>>(result.Data?.Items ?? new List<CouponUsage>());
        var pagedResult = new PagedResult<CouponUsageDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<CouponUsageDto>>.Success(pagedResult);
    }
}

public class GetCouponUsagesByCouponIdQueryHandler(
    ICouponUsageRepository repository,
    IMapper mapper,
    ILogger<GetCouponUsagesByCouponIdQueryHandler> logger
) : IRequestHandler<GetCouponUsagesByCouponIdQuery, Result<PagedResult<CouponUsageDto>>>
{
    public async Task<Result<PagedResult<CouponUsageDto>>> Handle(GetCouponUsagesByCouponIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting CouponUsages by CouponId: {CouponId}", request.CouponId);

        var result = await repository.GetByCouponIdAsync(request.CouponId, request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get CouponUsages by CouponId: {CouponId}", request.CouponId);
            return Result<PagedResult<CouponUsageDto>>.Failure(result.ErrorMessage ?? "Failed to get CouponUsages.");
        }

        var dtos = mapper.Map<List<CouponUsageDto>>(result.Data?.Items ?? new List<CouponUsage>());
        var pagedResult = new PagedResult<CouponUsageDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<CouponUsageDto>>.Success(pagedResult);
    }
}

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

        var dtos = mapper.Map<List<CouponUsageDto>>(result.Data?.Items ?? new List<CouponUsage>());
        var pagedResult = new PagedResult<CouponUsageDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<CouponUsageDto>>.Success(pagedResult);
    }
}

