using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Inventory;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Inventory;

public class PurchaseOrderItemRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<PurchaseOrderItemRepository> logger
) : BasePagedRepository<PurchaseOrderItemEntity, PurchaseOrderItem>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IPurchaseOrderItemRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("PurchaseOrderId", typeof(Guid)),
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("VariantId", typeof(Guid)),
            new SearchableField("Quantity", typeof(int)),
            new SearchableField("UnitCost", typeof(decimal)),
            new SearchableField("TotalCost", typeof(decimal))
        };
    }

    public override string GetDefaultSortField()
    {
        return "ProductId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "PurchaseOrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "VariantId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "Quantity", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UnitCost", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TotalCost", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "PurchaseOrderId", PurchaseOrderItemFields.PurchaseOrderId },
            { "ProductId", PurchaseOrderItemFields.ProductId },
            { "VariantId", PurchaseOrderItemFields.VariantId },
            { "Quantity", PurchaseOrderItemFields.Quantity },
            { "UnitCost", PurchaseOrderItemFields.UnitCost },
            { "TotalCost", PurchaseOrderItemFields.TotalCost }
        };
    }

    protected override EntityQuery<PurchaseOrderItemEntity> ApplySearch(EntityQuery<PurchaseOrderItemEntity> query, string searchTerm)
    {
        return query;
    }

    protected override EntityQuery<PurchaseOrderItemEntity> ApplySorting(EntityQuery<PurchaseOrderItemEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "quantity" => PurchaseOrderItemFields.Quantity,
            "unitcost" => PurchaseOrderItemFields.UnitCost,
            "totalcost" => PurchaseOrderItemFields.TotalCost,
            _ => PurchaseOrderItemFields.ProductId
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<PurchaseOrderItemEntity> ApplyDefaultSorting(EntityQuery<PurchaseOrderItemEntity> query)
    {
        return query.OrderBy(PurchaseOrderItemFields.ProductId.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return PurchaseOrderItemFields.PoiId;
    }

    protected override object GetEntityId(PurchaseOrderItemEntity entity, EntityField2 primaryKeyField)
    {
        return entity.PoiId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return PurchaseOrderItemFields.PoiId.In(ids);
    }

    // Implementation of IPurchaseOrderItemRepository methods
    public async Task<Result<PurchaseOrderItem?>> GetByIdAsync(Guid poiId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(PurchaseOrderItemFields.PoiId, poiId, "PurchaseOrderItem", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PagedResult<PurchaseOrderItem>>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("PurchaseOrderId", purchaseOrderId),
            null,
            SortDirection.Ascending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<PurchaseOrderItem>>> GetByProductIdAsync(Guid productId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("ProductId", productId),
            null,
            SortDirection.Ascending,
            cancellationToken
        );
    }

    public async Task<bool> CreateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<PurchaseOrderItemEntity>(item);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            // Update the item with the entity data
            Mapper.Map(entity, item);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrderItem created: {PoiId}", entity.PoiId);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating purchase order item");
            return false;
        }
    }

    public async Task<bool> UpdateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<PurchaseOrderItemEntity>(item);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrderItem updated: {PoiId}", entity.PoiId);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating purchase order item: {PoiId}", item.PoiId);
            return false;
        }
    }

    public async Task<bool> DeleteAsync(Guid poiId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new PurchaseOrderItemEntity(poiId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrderItem deleted: {PoiId}", poiId);
            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting purchase order item: {PoiId}", poiId);
            return false;
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid poiId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(PurchaseOrderItemFields.PoiId, poiId, cancellationToken);
    }

    public async Task<Result<bool>> DeleteByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(PurchaseOrderItemFields.PurchaseOrderId == purchaseOrderId);
            var rowsAffected = await adapter.DeleteEntitiesDirectlyAsync(typeof(PurchaseOrderItemEntity), bucket, cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("Deleted {Count} items from purchase order: {PurchaseOrderId}", rowsAffected, purchaseOrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting items by purchase order: {PurchaseOrderId}", purchaseOrderId);
            return Result<bool>.Failure("An error occurred while deleting purchase order items.");
        }
    }

    public async Task<Result<decimal>> GetTotalAmountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<PurchaseOrderItemEntity>()
                .Where(PurchaseOrderItemFields.PurchaseOrderId == purchaseOrderId)
                .Select(() => PurchaseOrderItemFields.TotalCost.Sum().As("TotalAmount"));
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            
            Logger.LogInformation("Total amount for purchase order {PurchaseOrderId}: {Total}", purchaseOrderId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting total amount by purchase order: {PurchaseOrderId}", purchaseOrderId);
            return Result<decimal>.Failure("An error occurred while getting total amount.");
        }
    }

    public async Task<Result<int>> GetItemCountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await CountByFieldAsync(PurchaseOrderItemFields.PurchaseOrderId, purchaseOrderId, cancellationToken);
    }

    public async Task<Result<PagedResult<PurchaseOrderItem>>> GetByVariantIdAsync(Guid variantId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("VariantId", variantId),
            null,
            SortDirection.Ascending,
            cancellationToken
        );
    }
}

