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

public class CouponUsageRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<CouponUsageRepository> logger
) : BasePagedRepository<CouponUsageEntity, CouponUsage>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), ICouponUsageRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("CouponId", typeof(Guid)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("DiscountAmount", typeof(decimal)),
            new SearchableField("UsedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "UsedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "CouponId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DiscountAmount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UsedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "couponid", CouponUsageFields.CouponId },
            { "userid", CouponUsageFields.UserId },
            { "orderid", CouponUsageFields.OrderId },
            { "discountamount", CouponUsageFields.DiscountAmount },
            { "usedat", CouponUsageFields.UsedAt }
        };
    }

    protected override EntityQuery<CouponUsageEntity> ApplySearch(EntityQuery<CouponUsageEntity> query, string searchTerm)
    {
        // CouponUsage không có text fields để search, chỉ có IDs
        return query;
    }

    protected override EntityQuery<CouponUsageEntity> ApplySorting(EntityQuery<CouponUsageEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "couponid" => CouponUsageFields.CouponId,
            "userid" => CouponUsageFields.UserId,
            "orderid" => CouponUsageFields.OrderId,
            "discountamount" => CouponUsageFields.DiscountAmount,
            "usedat" => CouponUsageFields.UsedAt,
            _ => CouponUsageFields.UsedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<CouponUsageEntity> ApplyDefaultSorting(EntityQuery<CouponUsageEntity> query)
    {
        return query.OrderBy(CouponUsageFields.UsedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return CouponUsageFields.UsageId;
    }

    protected override object GetEntityId(CouponUsageEntity entity, EntityField2 primaryKeyField)
    {
        return entity.UsageId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return CouponUsageFields.UsageId.In(ids);
    }

    public async Task<Result<CouponUsage?>> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(CouponUsageFields.UsageId, usageId, "CouponUsage", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PagedResult<CouponUsage>>> GetByCouponIdAsync(Guid couponId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("CouponId", couponId),
            CouponUsageFields.UsedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<CouponUsage>>> GetByUserIdAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("UserId", userId),
            CouponUsageFields.UsedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<CouponUsage>>> GetByOrderIdAsync(Guid orderId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("OrderId", orderId),
            CouponUsageFields.UsedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<CouponUsage>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await base.GetPagedAsync(request, cancellationToken);
    }

    public async Task<Result<CouponUsage>> CreateAsync(CouponUsage usage, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<CouponUsageEntity>(usage);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("CouponUsage*", cancellationToken);
            
            Logger.LogInformation("CouponUsage created: {UsageId}", entity.UsageId);
            var domainEntity = Mapper.Map<CouponUsage>(entity);
            return Result<CouponUsage>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating CouponUsage");
            return Result<CouponUsage>.Failure("An error occurred while creating CouponUsage.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(CouponUsage usage, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<CouponUsageEntity>(usage);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("CouponUsage*", cancellationToken);
            
            Logger.LogInformation("CouponUsage updated: {UsageId}", entity.UsageId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating CouponUsage: {UsageId}", usage.UsageId);
            return Result<bool>.Failure("An error occurred while updating CouponUsage.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new CouponUsageEntity(usageId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("CouponUsage*", cancellationToken);
            
            Logger.LogInformation("CouponUsage deleted: {UsageId}", usageId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting CouponUsage: {UsageId}", usageId);
            return Result<bool>.Failure("An error occurred while deleting CouponUsage.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(CouponUsageFields.UsageId, usageId, cancellationToken);
    }

    public async Task<Result<int>> GetUsageCountByCouponAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        return await CountByFieldAsync(CouponUsageFields.CouponId, couponId, cancellationToken);
    }

    public async Task<Result<int>> GetUsageCountByUserAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponUsageEntity>()
                .Where(CouponUsageFields.UserId == userId & CouponUsageFields.CouponId == couponId)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            Logger.LogInformation("Usage count for user {UserId} and coupon {CouponId}: {Count}", userId, couponId, count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting usage count for user {UserId} and coupon {CouponId}", userId, couponId);
            return Result<int>.Failure("An error occurred while getting usage count.");
        }
    }

    public async Task<Result<bool>> HasUserUsedCouponAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponUsageEntity>()
                .Where(CouponUsageFields.UserId == userId & CouponUsageFields.CouponId == couponId)
                .Limit(1)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if user {UserId} has used coupon {CouponId}", userId, couponId);
            return Result<bool>.Failure("An error occurred while checking coupon usage.");
        }
    }

    public async Task<Result<decimal>> GetTotalDiscountByCouponAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponUsageEntity>()
                .Where(CouponUsageFields.CouponId == couponId)
                .Select(() => CouponUsageFields.DiscountAmount.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            Logger.LogInformation("Total discount for coupon {CouponId}: {Total}", couponId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting total discount for coupon {CouponId}", couponId);
            return Result<decimal>.Failure("An error occurred while getting total discount.");
        }
    }

    public async Task<Result<decimal>> GetTotalDiscountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponUsageEntity>()
                .Where(CouponUsageFields.UserId == userId)
                .Select(() => CouponUsageFields.DiscountAmount.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            Logger.LogInformation("Total discount for user {UserId}: {Total}", userId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting total discount for user {UserId}", userId);
            return Result<decimal>.Failure("An error occurred while getting total discount.");
        }
    }

    public async Task<Result<PagedResult<CouponUsage>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithRangeFilter("UsedAt", fromDate, toDate),
            CouponUsageFields.UsedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }
}

