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

public class OrderItemRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderItemRepository> logger
) : BasePagedRepository<OrderItemEntity, OrderItem>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IOrderItemRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("ProductName", typeof(string)),
            new SearchableField("ProductSku", typeof(string)),
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("ProductVariantId", typeof(Guid)),
            new SearchableField("UnitPrice", typeof(decimal)),
            new SearchableField("Quantity", typeof(int)),
            new SearchableField("TotalPrice", typeof(decimal))
        };
    }

    public override string GetDefaultSortField()
    {
        return "OrderItemId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "ProductName", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductSku", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductVariantId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UnitPrice", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Quantity", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TotalPrice", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "productname", OrderItemFields.ProductName },
            { "productsku", OrderItemFields.ProductSku },
            { "orderid", OrderItemFields.OrderId },
            { "productid", OrderItemFields.ProductId },
            { "productvariantid", OrderItemFields.ProductVariantId },
            { "unitprice", OrderItemFields.UnitPrice },
            { "quantity", OrderItemFields.Quantity },
            { "totalprice", OrderItemFields.TotalPrice }
        };
    }

    protected override EntityQuery<OrderItemEntity> ApplySearch(EntityQuery<OrderItemEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            OrderItemFields.ProductName,
            OrderItemFields.ProductSku);

        return query.Where(predicate);
    }

    protected override EntityQuery<OrderItemEntity> ApplySorting(EntityQuery<OrderItemEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "productname" => OrderItemFields.ProductName,
            "productsku" => OrderItemFields.ProductSku,
            "orderid" => OrderItemFields.OrderId,
            "productid" => OrderItemFields.ProductId,
            "productvariantid" => OrderItemFields.ProductVariantId,
            "unitprice" => OrderItemFields.UnitPrice,
            "quantity" => OrderItemFields.Quantity,
            "totalprice" => OrderItemFields.TotalPrice,
            _ => OrderItemFields.OrderItemId
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderItemEntity> ApplyDefaultSorting(EntityQuery<OrderItemEntity> query)
    {
        return query.OrderBy(OrderItemFields.OrderItemId.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderItemFields.OrderItemId;
    }

    protected override object GetEntityId(OrderItemEntity entity, EntityField2 primaryKeyField)
    {
        return entity.OrderItemId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderItemFields.OrderItemId.In(ids);
    }

    public async Task<Result<OrderItem?>> GetByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderItemFields.OrderItemId, orderItemId, "OrderItem", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<OrderItem>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId == orderId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderItem>>(entities);
            
            return Result<List<OrderItem>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order items for order: {OrderId}", orderId);
            return Result<List<OrderItem>>.Failure("An error occurred while fetching order items.");
        }
    }

    public async Task<Result<PagedResult<OrderItem>>> GetByProductIdAsync(Guid productId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("ProductId", productId),
            OrderItemFields.OrderItemId.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<bool>> CreateAsync(OrderItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderItemEntity>(item);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item created: {OrderItemId}", entity.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order item");
            return Result<bool>.Failure("An error occurred while creating order item.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(OrderItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderItemEntity>(item);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item updated: {OrderItemId}", entity.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order item: {OrderItemId}", item.OrderItemId);
            return Result<bool>.Failure("An error occurred while updating order item.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderItemEntity(orderItemId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item deleted: {OrderItemId}", orderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order item: {OrderItemId}", orderItemId);
            return Result<bool>.Failure("An error occurred while deleting order item.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(OrderItemFields.OrderItemId, orderItemId, cancellationToken);
    }

    public async Task<Result<bool>> DeleteByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId == orderId);
            
            var entities = new EntityCollection<OrderItemEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            
            if (entities.Count > 0)
            {
                await adapter.DeleteEntityCollectionAsync(entities, cancellationToken);
            }
            
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order items deleted for order: {OrderId}", orderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order items for order: {OrderId}", orderId);
            return Result<bool>.Failure("An error occurred while deleting order items.");
        }
    }

    public async Task<Result<decimal>> GetTotalAmountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId == orderId)
                .Select(() => OrderItemFields.TotalPrice.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating total amount for order: {OrderId}", orderId);
            return Result<decimal>.Failure("An error occurred while calculating total amount.");
        }
    }

    public async Task<Result<int>> GetItemCountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId == orderId)
                .Select(() => OrderItemFields.Quantity.Sum());
            
            var count = await adapter.FetchScalarAsync<int?>(query, cancellationToken);
            return Result<int>.Success(count ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating item count for order: {OrderId}", orderId);
            return Result<int>.Failure("An error occurred while calculating item count.");
        }
    }

    public async Task<Result<PagedResult<OrderItem>>> GetByVariantIdAsync(Guid variantId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("ProductVariantId", variantId),
            OrderItemFields.OrderItemId.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderItem>>> GetBySkuAsync(string sku, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("ProductSku", sku),
            OrderItemFields.OrderItemId.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }
}

