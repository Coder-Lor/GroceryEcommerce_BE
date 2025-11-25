using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class AuditLogRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<AuditLogRepository> logger
) : BasePagedRepository<AuditLogEntity, AuditLog>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IAuditLogRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Action", typeof(string)),
            new SearchableField("Entity", typeof(string)),
            new SearchableField("Detail", typeof(string)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("CreatedAt", typeof(DateTime))
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
            new FieldMapping { FieldName = "Action", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Entity", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Detail", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "Action", AuditLogFields.Action },
            { "Entity", AuditLogFields.Entity },
            { "Detail", AuditLogFields.Detail },
            { "UserId", AuditLogFields.UserId },
            { "CreatedAt", AuditLogFields.CreatedAt },
            { "EntityId", AuditLogFields.EntityId },
            { "AuditId", AuditLogFields.AuditId }
        };
    }

    protected override EntityQuery<AuditLogEntity> ApplySearch(EntityQuery<AuditLogEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        
        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            AuditLogFields.Action,
            AuditLogFields.Entity,
            AuditLogFields.Detail);

        return query.Where(predicate);
    }

    protected override EntityQuery<AuditLogEntity> ApplySorting(EntityQuery<AuditLogEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;

        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<AuditLogEntity> ApplyDefaultSorting(EntityQuery<AuditLogEntity> query)
    {
        return query.OrderBy(AuditLogFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return AuditLogFields.AuditId;
    }

    protected override object GetEntityId(AuditLogEntity entity, EntityField2 primaryKeyField)
    {
        return entity.AuditId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    protected override async Task<IList<AuditLogEntity>> FetchEntitiesAsync(EntityQuery<AuditLogEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<AuditLogEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "action" => AuditLogFields.Action,
            "entity" => AuditLogFields.Entity,
            "createdat" => AuditLogFields.CreatedAt,
            "userid" => AuditLogFields.UserId,
            _ => AuditLogFields.CreatedAt
        };
    }

    public async Task<Result<AuditLog?>> GetByIdAsync(Guid auditId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (auditId == Guid.Empty)
                return Result<AuditLog?>.Failure("Invalid audit id.");

            var cacheKey = $"AuditLog_{auditId}";
            var cached = await CacheService.GetAsync<AuditLog>(cacheKey, cancellationToken);
            if (cached != null)
            {
                Logger.LogInformation("AuditLog fetched from cache: {AuditId}", auditId);
                return Result<AuditLog?>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.AuditLog.Where(AuditLogFields.AuditId == auditId).WithPath(AuditLogEntity.PrefetchPathUser);
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<AuditLog?>.Success(null);

            var domain = Mapper.Map<AuditLog>(entity);
            await CacheService.SetAsync(cacheKey, domain, TimeSpan.FromHours(1), cancellationToken);
            return Result<AuditLog?>.Success(domain);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting audit log by id: {AuditId}", auditId);
            return Result<AuditLog?>.Failure("An error occurred while retrieving audit log.");
        }
    }

    public async Task<Result<AuditLog>> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<AuditLogEntity>(auditLog);
            entity.AuditId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsNew = true;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
                return Result<AuditLog>.Failure("Failed to create audit log.");

            var created = Mapper.Map<AuditLog>(entity);
            await CacheService.RemoveAsync("Recent_AuditLogs", cancellationToken);
            Logger.LogInformation("Audit log created: {AuditId}", entity.AuditId);
            return Result<AuditLog>.Success(created);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating audit log");
            return Result<AuditLog>.Failure("An error occurred while creating audit log.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid auditId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (auditId == Guid.Empty)
                return Result<bool>.Failure("Invalid audit id.");

            var entity = new AuditLogEntity(auditId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
                return Result<bool>.Failure("Audit log not found or failed to delete.");

            await CacheService.RemoveAsync($"AuditLog_{auditId}", cancellationToken);
            await CacheService.RemoveAsync("Recent_AuditLogs", cancellationToken);
            Logger.LogInformation("Audit log deleted: {AuditId}", auditId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting audit log: {AuditId}", auditId);
            return Result<bool>.Failure("An error occurred while deleting audit log.");
        }
    }

    public async Task<Result<PagedResult<AuditLog>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("UserId", userId), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<AuditLog>>> GetByEntityAsync(PagedRequest request, string entity, Guid entityId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => {
            r.WithFilter("Entity", entity);
            if (entityId != Guid.Empty) r.WithFilter("EntityId", entityId);
        }, "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<AuditLog>>> GetByActionAsync(PagedRequest request, string action, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Action", action), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<AuditLog>>> GetByDateRangeAsync(PagedRequest request, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithRangeFilter("CreatedAt", fromDate, toDate), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<AuditLog>>> GetByUserAndDateRangeAsync(PagedRequest request, Guid userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => {
            r.WithRangeFilter("CreatedAt", fromDate, toDate);
            if (userId != Guid.Empty) r.WithFilter("UserId", userId);
        }, "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<AuditLog>>> GetRecentLogsAsync(PagedRequest request, int count = 100, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithPaging(1, count), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<int>> GetLogCountByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await CountByFieldAsync(AuditLogFields.UserId, userId, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting audit logs by user: {UserId}", userId);
            return Result<int>.Failure("An error occurred while counting audit logs by user.");
        }
    }

    public async Task<Result<int>> GetLogCountByActionAsync(string action, CancellationToken cancellationToken = default)
    {
        try
        {
            return await CountByFieldAsync(AuditLogFields.Action, action, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting audit logs by action: {Action}", action);
            return Result<int>.Failure("An error occurred while counting audit logs by action.");
        }
    }

    public async Task<Result<Dictionary<string, int>>> GetActionStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var qf = new QueryFactory();
            var query = qf.AuditLog.Where(AuditLogFields.CreatedAt >= fromDate & AuditLogFields.CreatedAt <= toDate);
            var adapter = GetAdapter();
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var list = Mapper.Map<List<AuditLog>>(entities);

            var stats = list
                .Where(x => !string.IsNullOrWhiteSpace(x.Action))
                .GroupBy(x => x.Action)
                .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

            return Result<Dictionary<string, int>>.Success(stats);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting action statistics between {From} and {To}", fromDate, toDate);
            return Result<Dictionary<string, int>>.Failure("An error occurred while retrieving action statistics.");
        }
    }
}