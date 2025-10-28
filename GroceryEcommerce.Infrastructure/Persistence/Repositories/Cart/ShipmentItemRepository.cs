using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Sales;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Cart;

public class ShipmentItemRepository(
    DataAccessAdapter adapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ShipmentItemRepository> logger

): BasePagedRepository<ShipmentItemEntity, ShipmentItem>(adapter, unitOfWorkService, mapper, cacheService, logger), IShipmentItemRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("ShipmentId", typeof(Guid)),
            new SearchableField("OrderItemId", typeof(Guid)),
            new SearchableField("Quantity", typeof(int))
        };
    }

    public override string GetDefaultSortField()
    {
        return "ShipmentId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "ShipmentItemId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "ShipmentId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderItemId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Quantity", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["shipmentitemid"] = ShipmentItemFields.ShipmentItemId,
            ["shipmentid"] = ShipmentItemFields.ShipmentId,
            ["orderitemid"] = ShipmentItemFields.OrderItemId,
            ["quantity"] = ShipmentItemFields.Quantity
        };
    }

    protected override EntityQuery<ShipmentItemEntity> ApplySearch(EntityQuery<ShipmentItemEntity> query, string searchTerm)
    {
        return query;
    }

    protected override EntityQuery<ShipmentItemEntity> ApplySorting(EntityQuery<ShipmentItemEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<ShipmentItemEntity> ApplyDefaultSorting(EntityQuery<ShipmentItemEntity> query)
    {
        return query.OrderBy(ShipmentItemFields.ShipmentId.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ShipmentItemFields.ShipmentItemId;
    }

    protected override object GetEntityId(ShipmentItemEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ShipmentItemId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<ShipmentItem?>> GetByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(ShipmentItemFields.ShipmentItemId, shipmentItemId, "ShipmentItem", TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task<Result<PagedResult<ShipmentItem>>> GetByShipmentIdAsync(PagedRequest request, Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("ShipmentId", shipmentId), cancellationToken: cancellationToken);
    }

    public Task<Result<PagedResult<ShipmentItem>>> GetByOrderItemIdAsync(PagedRequest request, Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("OrderItemId", orderItemId), cancellationToken: cancellationToken);
    }

    public async Task<Result<ShipmentItem>> CreateAsync(ShipmentItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentItemEntity>(item);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<ShipmentItem>.Success(item) : Result<ShipmentItem>.Failure("Create shipment item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating shipment item");
            return Result<ShipmentItem>.Failure("Error creating shipment item");
        }
    }

    public async Task<Result<bool>> UpdateAsync(ShipmentItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentItemEntity>(item);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update shipment item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating shipment item");
            return Result<bool>.Failure("Error updating shipment item");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (shipmentItemId == Guid.Empty) return Result<bool>.Failure("Invalid shipmentItemId");
            var adapter = GetAdapter();
            var entity = new ShipmentItemEntity(shipmentItemId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete shipment item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting shipment item");
            return Result<bool>.Failure("Error deleting shipment item");
        }
    }

    public Task<Result<bool>> ExistsAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        return ExistsByCountAsync(ShipmentItemFields.ShipmentItemId, shipmentItemId, cancellationToken);
    }

    public async Task<Result<bool>> DeleteByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (shipmentId == Guid.Empty) return Result<bool>.Failure("Invalid shipmentId");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(ShipmentItemFields.ShipmentId == shipmentId);
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(ShipmentItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting shipment items by shipment");
            return Result<bool>.Failure("Error deleting shipment items by shipment");
        }
    }

    public async Task<Result<int>> GetItemCountByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentItemEntity>().Where(ShipmentItemFields.ShipmentId == shipmentId).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting shipment items");
            return Result<int>.Failure("Error counting shipment items");
        }
    }

    public async Task<Result<int>> GetTotalQuantityByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentItemEntity>().Where(ShipmentItemFields.ShipmentId == shipmentId);
            var items = await FetchEntitiesAsync(query, adapter, cancellationToken);
            var total = 0;
            foreach (var e in items)
            {
                total += e.Quantity;
            }
            return Result<int>.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error summing shipment items quantity");
            return Result<int>.Failure("Error summing shipment items quantity");
        }
    }

    public Task<Result<PagedResult<ShipmentItem>>> GetByOrderIdAsync(PagedRequest request, Guid orderId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("OrderId", orderId), cancellationToken: cancellationToken);
    }
}