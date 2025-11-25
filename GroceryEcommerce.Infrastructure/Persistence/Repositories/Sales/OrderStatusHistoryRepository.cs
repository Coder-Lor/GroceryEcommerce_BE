using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Sales;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Sales;

public class OrderStatusHistoryRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderStatusHistoryRepository> logger
) : BasePagedRepository<OrderStatusHistoryEntity, OrderStatusHistory>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IOrderStatusHistoryRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("FromStatus", typeof(short)),
            new SearchableField("ToStatus", typeof(short)),
            new SearchableField("Comment", typeof(string)),
            new SearchableField("CreatedBy", typeof(Guid)),
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
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "FromStatus", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ToStatus", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Comment", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedBy", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "orderid", OrderStatusHistoryFields.OrderId },
            { "fromstatus", OrderStatusHistoryFields.FromStatus },
            { "tostatus", OrderStatusHistoryFields.ToStatus },
            { "comment", OrderStatusHistoryFields.Comment },
            { "createdby", OrderStatusHistoryFields.CreatedBy },
            { "createdat", OrderStatusHistoryFields.CreatedAt }
        };
    }

    protected override EntityQuery<OrderStatusHistoryEntity> ApplySearch(EntityQuery<OrderStatusHistoryEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        return query.Where(SearchPredicateBuilder.BuildContainsPredicate(searchTerm, OrderStatusHistoryFields.Comment));
    }

    protected override EntityQuery<OrderStatusHistoryEntity> ApplySorting(EntityQuery<OrderStatusHistoryEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "orderid" => OrderStatusHistoryFields.OrderId,
            "fromstatus" => OrderStatusHistoryFields.FromStatus,
            "tostatus" => OrderStatusHistoryFields.ToStatus,
            "createdby" => OrderStatusHistoryFields.CreatedBy,
            "createdat" => OrderStatusHistoryFields.CreatedAt,
            _ => OrderStatusHistoryFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderStatusHistoryEntity> ApplyDefaultSorting(EntityQuery<OrderStatusHistoryEntity> query)
    {
        return query.OrderBy(OrderStatusHistoryFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderStatusHistoryFields.HistoryId;
    }

    protected override object GetEntityId(OrderStatusHistoryEntity entity, EntityField2 primaryKeyField)
    {
        return entity.HistoryId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderStatusHistoryFields.HistoryId.In(ids);
    }

    public async Task<Result<OrderStatusHistory?>> GetByIdAsync(Guid historyId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderStatusHistoryFields.HistoryId, historyId, "OrderStatusHistory", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<OrderStatusHistory>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderStatusHistoryEntity>()
                .Where(OrderStatusHistoryFields.OrderId == orderId)
                .OrderBy(OrderStatusHistoryFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderStatusHistory>>(entities);
            
            return Result<List<OrderStatusHistory>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order status history for order: {OrderId}", orderId);
            return Result<List<OrderStatusHistory>>.Failure("An error occurred while fetching order status history.");
        }
    }

    public async Task<Result<bool>> CreateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderStatusHistoryEntity>(history);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderStatusHistory*", cancellationToken);
            
            Logger.LogInformation("Order status history created: {HistoryId}", entity.HistoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order status history");
            return Result<bool>.Failure("An error occurred while creating order status history.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderStatusHistoryEntity>(history);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderStatusHistory*", cancellationToken);
            
            Logger.LogInformation("Order status history updated: {HistoryId}", entity.HistoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order status history: {HistoryId}", history.HistoryId);
            return Result<bool>.Failure("An error occurred while updating order status history.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid historyId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderStatusHistoryEntity(historyId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderStatusHistory*", cancellationToken);
            
            Logger.LogInformation("Order status history deleted: {HistoryId}", historyId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order status history: {HistoryId}", historyId);
            return Result<bool>.Failure("An error occurred while deleting order status history.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid historyId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(OrderStatusHistoryFields.HistoryId, historyId, cancellationToken);
    }

    public async Task<Result<PagedResult<OrderStatusHistory>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("ToStatus", status),
            OrderStatusHistoryFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderStatusHistory>>> GetByUserAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("CreatedBy", userId),
            OrderStatusHistoryFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderStatusHistory>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithRangeFilter("CreatedAt", fromDate, toDate),
            OrderStatusHistoryFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<OrderStatusHistory?>> GetLatestByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderStatusHistoryEntity>()
                .Where(OrderStatusHistoryFields.OrderId == orderId)
                .OrderBy(OrderStatusHistoryFields.CreatedAt.Descending())
                .Limit(1);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<OrderStatusHistory?>.Success(null);
            
            var domainEntity = Mapper.Map<OrderStatusHistory>(entity);
            return Result<OrderStatusHistory?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching latest status history for order: {OrderId}", orderId);
            return Result<OrderStatusHistory?>.Failure("An error occurred while fetching latest status history.");
        }
    }

    public async Task<Result<bool>> AddStatusChangeAsync(Guid orderId, short fromStatus, short toStatus, string? comment, Guid createdBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var history = new OrderStatusHistory
            {
                HistoryId = Guid.NewGuid(),
                OrderId = orderId,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                Comment = comment,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            
            return await CreateAsync(history, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding status change for order: {OrderId}", orderId);
            return Result<bool>.Failure("An error occurred while adding status change.");
        }
    }
}

