using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Cart;

public class AbandonedCartRepository(
    DataAccessAdapter adapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<AbandonedCartRepository> logger
): BasePagedRepository<AbandonedCartEntity, AbandonedCart>(adapter, unitOfWorkService, mapper, cacheService, logger), IAbandonedCartRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("CartId", typeof(Guid)),
            new SearchableField("Notified", typeof(bool)),
            new SearchableField("AbandonedAt", typeof(DateTime))
        };
    }

    public override string? GetDefaultSortField()
    {
        return "AbandonedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "AbandonedId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CartId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Notified", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AbandonedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["abandonedid"] = AbandonedCartFields.AbandonedId,
            ["userid"] = AbandonedCartFields.UserId,
            ["cartid"] = AbandonedCartFields.CartId,
            ["notified"] = AbandonedCartFields.Notified,
            ["abandonedat"] = AbandonedCartFields.AbandonedAt
        };
    }

    protected override EntityQuery<AbandonedCartEntity> ApplySearch(EntityQuery<AbandonedCartEntity> query, string searchTerm)
    {
        return query;
    }

    protected override EntityQuery<AbandonedCartEntity> ApplySorting(EntityQuery<AbandonedCartEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<AbandonedCartEntity> ApplyDefaultSorting(EntityQuery<AbandonedCartEntity> query)
    {
        return query.OrderBy(AbandonedCartFields.AbandonedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return AbandonedCartFields.AbandonedId;
    }

    protected override object GetEntityId(AbandonedCartEntity entity, EntityField2 primaryKeyField)
    {
        return entity.AbandonedId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<AbandonedCart?>> GetByIdAsync(Guid abandonedId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(AbandonedCartFields.AbandonedId, abandonedId, "AbandonedCart", TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task<Result<PagedResult<AbandonedCart>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("UserId", userId), cancellationToken: cancellationToken);
    }

    public Task<Result<AbandonedCart>> CreateAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        return CreateInternalAsync(abandonedCart, cancellationToken);
    }

    public Task<Result<bool>> UpdateAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(abandonedCart, cancellationToken);
    }

    public Task<Result<bool>> DeleteAsync(Guid abandonedId, CancellationToken cancellationToken = default)
    {
        return DeleteInternalAsync(abandonedId, cancellationToken);
    }

    public Task<Result<PagedResult<AbandonedCart>>> GetUnnotifiedCartsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("Notified", false), cancellationToken: cancellationToken);
    }

    public Task<Result<PagedResult<AbandonedCart>>> GetUnnotifiedCartsByTimeRangeAsync(PagedRequest request, DateTime fromDate, DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithRangeFilter("AbandonedAt", fromDate, toDate), cancellationToken: cancellationToken);
    }

    public async Task<Result<bool>> MarkAsNotifiedAsync(Guid abandonedId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new AbandonedCartEntity(abandonedId) { Notified = true };
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Mark notified failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mark abandoned as notified");
            return Result<bool>.Failure("Error mark as notified");
        }
    }

    public async Task<Result<bool>> MarkAsNotifiedAsync(List<Guid> abandonedIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var tasks = new List<Task<Result<bool>>>();
            foreach (var id in abandonedIds)
            {
                tasks.Add(MarkAsNotifiedAsync(id, cancellationToken));
            }
            await Task.WhenAll(tasks);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mark list abandoned as notified");
            return Result<bool>.Failure("Error mark list as notified");
        }
    }

    public Task<Result<PagedResult<AbandonedCart>>> GetByDateRangeAsync(PagedRequest request, DateTime fromDate, DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithRangeFilter("AbandonedAt", fromDate, toDate), cancellationToken: cancellationToken);
    }

    public async Task<Result<int>> GetAbandonedCartCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<AbandonedCartEntity>().Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting abandoned carts");
            return Result<int>.Failure("Error counting abandoned carts");
        }
    }

    public async Task<Result<int>> GetUnnotifiedCartCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<AbandonedCartEntity>().Where(AbandonedCartFields.Notified == false).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting unnotified carts");
            return Result<int>.Failure("Error counting unnotified carts");
        }
    }

    private async Task<Result<AbandonedCart>> CreateInternalAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<AbandonedCartEntity>(abandonedCart);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<AbandonedCart>.Success(abandonedCart) : Result<AbandonedCart>.Failure("Create abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating abandoned cart");
            return Result<AbandonedCart>.Failure("Error creating abandoned cart");
        }
    }

    private async Task<Result<bool>> UpdateInternalAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<AbandonedCartEntity>(abandonedCart);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating abandoned cart");
            return Result<bool>.Failure("Error updating abandoned cart");
        }
    }

    private async Task<Result<bool>> DeleteInternalAsync(Guid abandonedId, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new AbandonedCartEntity(abandonedId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting abandoned cart");
            return Result<bool>.Failure("Error deleting abandoned cart");
        }
    }
}