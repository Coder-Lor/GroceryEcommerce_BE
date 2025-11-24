using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Commands;
using GroceryEcommerce.Application.Features.Marketing.RewardPoint.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using GroceryEcommerce.Domain.Entities.Marketing;
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

        var rewardPoint = new RewardPoint
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

        var dto = mapper.Map<RewardPointDto>(result.Data);
        logger.LogInformation("RewardPoint created: {RewardId}", result.Data.RewardId);
        return Result<RewardPointDto>.Success(dto);
    }
}

public class UpdateRewardPointCommandHandler(
    IRewardPointRepository repository,
    IMapper mapper,
    ILogger<UpdateRewardPointCommandHandler> logger
) : IRequestHandler<UpdateRewardPointCommand, Result<RewardPointDto>>
{
    public async Task<Result<RewardPointDto>> Handle(UpdateRewardPointCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating RewardPoint: {RewardPointId}", request.RewardPointId);

        var existingResult = await repository.GetByIdAsync(request.RewardPointId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            return Result<RewardPointDto>.Failure("RewardPoint not found.");
        }

        var rewardPoint = existingResult.Data;
        rewardPoint.Points = request.Points;
        rewardPoint.Reason = request.Reason;
        rewardPoint.ExpiresAt = request.ExpiresAt;

        var updateResult = await repository.UpdateAsync(rewardPoint, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update RewardPoint: {RewardPointId}", request.RewardPointId);
            return Result<RewardPointDto>.Failure(updateResult.ErrorMessage ?? "Failed to update RewardPoint.");
        }

        var updatedResult = await repository.GetByIdAsync(request.RewardPointId, cancellationToken);
        if (!updatedResult.IsSuccess || updatedResult.Data == null)
        {
            return Result<RewardPointDto>.Failure("Failed to retrieve updated RewardPoint.");
        }

        var dto = mapper.Map<RewardPointDto>(updatedResult.Data);
        logger.LogInformation("RewardPoint updated: {RewardPointId}", request.RewardPointId);
        return Result<RewardPointDto>.Success(dto);
    }
}

public class DeleteRewardPointCommandHandler(
    IRewardPointRepository repository,
    ILogger<DeleteRewardPointCommandHandler> logger
) : IRequestHandler<DeleteRewardPointCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteRewardPointCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting RewardPoint: {RewardPointId}", request.RewardPointId);

        var result = await repository.DeleteAsync(request.RewardPointId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete RewardPoint: {RewardPointId}", request.RewardPointId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete RewardPoint.");
        }

        logger.LogInformation("RewardPoint deleted: {RewardPointId}", request.RewardPointId);
        return Result<bool>.Success(true);
    }
}

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

        var dtos = mapper.Map<List<RewardPointDto>>(result.Data ?? new List<RewardPoint>());
        return Result<List<RewardPointDto>>.Success(dtos);
    }
}

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

        var dtos = mapper.Map<List<RewardPointDto>>(result.Data?.Items ?? new List<RewardPoint>());
        var pagedResult = new PagedResult<RewardPointDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<RewardPointDto>>.Success(pagedResult);
    }
}

