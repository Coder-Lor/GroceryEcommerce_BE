using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Marketing;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Marketing;

public class RewardPointRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<RewardPointRepository> logger
) : BasePagedRepository<RewardPointEntity, RewardPoint>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IRewardPointRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("RewardId", typeof(Guid)),
            new SearchableField("Points", typeof(int)),
            new SearchableField("Reason", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("ExpiresAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "CreatedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "RewardId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Points", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Reason", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ExpiresAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "userid", RewardPointFields.UserId },
            { "rewardid", RewardPointFields.RewardId },
            { "points", RewardPointFields.Points },
            { "reason", RewardPointFields.Reason },
            { "createdat", RewardPointFields.CreatedAt },
            { "expiresat", RewardPointFields.ExpiresAt }
        };
    }

    protected override EntityQuery<RewardPointEntity> ApplySearch(EntityQuery<RewardPointEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(searchTerm, RewardPointFields.Reason);
        return query.Where(predicate);
    }

    protected override EntityQuery<RewardPointEntity> ApplySorting(EntityQuery<RewardPointEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "userid" => RewardPointFields.UserId,
            "rewardid" => RewardPointFields.RewardId,
            "points" => RewardPointFields.Points,
            "reason" => RewardPointFields.Reason,
            "createdat" => RewardPointFields.CreatedAt,
            "expiresat" => RewardPointFields.ExpiresAt,
            _ => RewardPointFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<RewardPointEntity> ApplyDefaultSorting(EntityQuery<RewardPointEntity> query)
    {
        return query.OrderBy(RewardPointFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return RewardPointFields.RewardId;
    }

    protected override object GetEntityId(RewardPointEntity entity, EntityField2 primaryKeyField)
    {
        return entity.RewardId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return RewardPointFields.RewardId.In(ids);
    }

    public async Task<Result<RewardPoint?>> GetByIdAsync(Guid rewardPointId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(RewardPointFields.RewardId, rewardPointId, "RewardPoint", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<RewardPoint>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<RewardPointEntity>()
                .Where(RewardPointFields.UserId == userId)
                .OrderBy(RewardPointFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<RewardPoint>>(entities);
            
            return Result<List<RewardPoint>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting RewardPoints for user {UserId}", userId);
            return Result<List<RewardPoint>>.Failure("An error occurred while fetching RewardPoints.");
        }
    }

    public async Task<Result<PagedResult<RewardPoint>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await base.GetPagedAsync(request, cancellationToken);
    }

    public async Task<Result<RewardPoint>> CreateAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<RewardPointEntity>(rewardPoint);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("RewardPoint*", cancellationToken);
            
            Logger.LogInformation("RewardPoint created: {RewardId}", entity.RewardId);
            var domainEntity = Mapper.Map<RewardPoint>(entity);
            return Result<RewardPoint>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating RewardPoint");
            return Result<RewardPoint>.Failure("An error occurred while creating RewardPoint.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<RewardPointEntity>(rewardPoint);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("RewardPoint*", cancellationToken);
            
            Logger.LogInformation("RewardPoint updated: {RewardId}", entity.RewardId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating RewardPoint: {RewardId}", rewardPoint.RewardId);
            return Result<bool>.Failure("An error occurred while updating RewardPoint.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid rewardPointId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new RewardPointEntity(rewardPointId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("RewardPoint*", cancellationToken);
            
            Logger.LogInformation("RewardPoint deleted: {RewardPointId}", rewardPointId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting RewardPoint: {RewardPointId}", rewardPointId);
            return Result<bool>.Failure("An error occurred while deleting RewardPoint.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid rewardPointId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(RewardPointFields.RewardId, rewardPointId, cancellationToken);
    }

    public async Task<Result<decimal>> GetTotalPointsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<RewardPointEntity>()
                .Where(RewardPointFields.UserId == userId)
                .Select(() => RewardPointFields.Points.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            Logger.LogInformation("Total points for user {UserId}: {Total}", userId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting total points for user {UserId}", userId);
            return Result<decimal>.Failure("An error occurred while getting total points.");
        }
    }

    public async Task<Result<decimal>> GetAvailablePointsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var now = DateTime.UtcNow;
            var query = qf.Create<RewardPointEntity>()
                .Where((RewardPointFields.UserId == userId).And(
                       RewardPointFields.ExpiresAt.IsNull().Or(RewardPointFields.ExpiresAt > now)))
                .Select(() => RewardPointFields.Points.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            Logger.LogInformation("Available points for user {UserId}: {Total}", userId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);                             
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting available points for user {UserId}", userId);
            return Result<decimal>.Failure("An error occurred while getting available points.");
        }
    }

    public async Task<Result<List<RewardPoint>>> GetPointsByTypeAsync(Guid userId, short pointType, CancellationToken cancellationToken = default)
    {
        try
        {
            // Note: Domain entity doesn't have PointType field, so this might need adjustment
            // For now, returning all points for the user
            var result = await GetByUserIdAsync(userId, cancellationToken);
            if (!result.IsSuccess)
            {
                return Result<List<RewardPoint>>.Failure(result.ErrorMessage ?? "Failed to get points.");
            }
            
            return Result<List<RewardPoint>>.Success(result.Data ?? new List<RewardPoint>());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting points by type for user {UserId}", userId);
            return Result<List<RewardPoint>>.Failure("An error occurred while getting points by type.");
        }
    }

    public async Task<Result<List<RewardPoint>>> GetExpiringPointsAsync(DateTime expiryDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<RewardPointEntity>()
                .Where(RewardPointFields.ExpiresAt <= expiryDate)
                .OrderBy(RewardPointFields.ExpiresAt.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<RewardPoint>>(entities);
            
            return Result<List<RewardPoint>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting expiring points");
            return Result<List<RewardPoint>>.Failure("An error occurred while getting expiring points.");
        }
    }

    public async Task<Result<bool>> AddPointsAsync(Guid userId, decimal points, short pointType, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var rewardPoint = new RewardPoint
            {
                RewardId = Guid.NewGuid(),
                UserId = userId,
                Points = (int)points,
                Reason = description,
                CreatedAt = DateTime.UtcNow
            };
            
            var result = await CreateAsync(rewardPoint, cancellationToken);
            return Result<bool>.Success(result.IsSuccess);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding points for user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while adding points.");
        }
    }

    public async Task<Result<bool>> DeductPointsAsync(Guid userId, decimal points, string description, CancellationToken cancellationToken = default)
    {
        try
        {
            var rewardPoint = new RewardPoint
            {
                RewardId = Guid.NewGuid(),
                UserId = userId,
                Points = -(int)points, // Negative points for deduction
                Reason = description,
                CreatedAt = DateTime.UtcNow
            };
            
            var result = await CreateAsync(rewardPoint, cancellationToken);
            return Result<bool>.Success(result.IsSuccess);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deducting points for user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while deducting points.");
        }
    }

    public async Task<Result<bool>> ExpirePointsAsync(Guid userId, decimal points, CancellationToken cancellationToken = default)
    {
        try
        {
            // This would typically mark points as expired or create an expiration record
            // For now, we'll create a deduction record with expiration reason
            return await DeductPointsAsync(userId, points, "Points expired", cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error expiring points for user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while expiring points.");
        }
    }
}

