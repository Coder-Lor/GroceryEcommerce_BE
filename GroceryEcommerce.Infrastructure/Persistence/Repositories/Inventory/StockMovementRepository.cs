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

public class StockMovementRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<StockMovementRepository> logger
) : BasePagedRepository<StockMovementEntity, StockMovement>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IStockMovementRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("ProductVariantId", typeof(Guid)),
            new SearchableField("WarehouseId", typeof(Guid)),
            new SearchableField("MovementType", typeof(short)),
            new SearchableField("Quantity", typeof(int)),
            new SearchableField("Reason", typeof(string)),
            new SearchableField("ReferenceType", typeof(short)),
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
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "ProductVariantId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "WarehouseId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "MovementType", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Quantity", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Reason", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "ReferenceType", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "ProductId", StockMovementFields.ProductId },
            { "ProductVariantId", StockMovementFields.ProductVariantId },
            { "WarehouseId", StockMovementFields.WarehouseId },
            { "MovementType", StockMovementFields.MovementType },
            { "Quantity", StockMovementFields.Quantity },
            { "Reason", StockMovementFields.Reason },
            { "ReferenceType", StockMovementFields.ReferenceType },
            { "CreatedAt", StockMovementFields.CreatedAt }
        };
    }

    protected override EntityQuery<StockMovementEntity> ApplySearch(EntityQuery<StockMovementEntity> query, string searchTerm)
    {
        return query.Where(StockMovementFields.Reason.Contains(searchTerm));
    }

    protected override EntityQuery<StockMovementEntity> ApplySorting(EntityQuery<StockMovementEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "movementtype" => StockMovementFields.MovementType,
            "quantity" => StockMovementFields.Quantity,
            "referencetype" => StockMovementFields.ReferenceType,
            "createdat" => StockMovementFields.CreatedAt,
            _ => StockMovementFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<StockMovementEntity> ApplyDefaultSorting(EntityQuery<StockMovementEntity> query)
    {
        return query.OrderBy(StockMovementFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return StockMovementFields.MovementId;
    }

    protected override object GetEntityId(StockMovementEntity entity, EntityField2 primaryKeyField)
    {
        return entity.MovementId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return StockMovementFields.MovementId.In(ids);
    }

    // Implementation of IStockMovementRepository methods
    public async Task<Result<StockMovement?>> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(StockMovementFields.MovementId, movementId, "StockMovement", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PagedResult<StockMovement>>> GetByProductIdAsync(Guid productId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("ProductId", productId),
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<StockMovement>>> GetByWarehouseIdAsync(Guid warehouseId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("WarehouseId", warehouseId),
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<StockMovement>> CreateAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<StockMovementEntity>(movement);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            var domainEntity = Mapper.Map<StockMovement>(entity);
            await CacheService.RemoveByPatternAsync("StockMovement*", cancellationToken);
            
            Logger.LogInformation("StockMovement created: {MovementId}", entity.MovementId);
            return Result<StockMovement>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating stock movement");
            return Result<StockMovement>.Failure("An error occurred while creating the stock movement.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(StockMovement movement, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<StockMovementEntity>(movement);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("StockMovement*", cancellationToken);
            
            Logger.LogInformation("StockMovement updated: {MovementId}", entity.MovementId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating stock movement: {MovementId}", movement.MovementId);
            return Result<bool>.Failure("An error occurred while updating the stock movement.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new StockMovementEntity(movementId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("StockMovement*", cancellationToken);
            
            Logger.LogInformation("StockMovement deleted: {MovementId}", movementId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting stock movement: {MovementId}", movementId);
            return Result<bool>.Failure("An error occurred while deleting the stock movement.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid movementId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(StockMovementFields.MovementId, movementId, cancellationToken);
    }

    public async Task<Result<PagedResult<StockMovement>>> GetByDateRangeAsync(PagedRequest pagedRequest, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("CreatedAt", fromDate, FilterOperator.GreaterThanOrEqual)
                      .WithFilter("CreatedAt", toDate, FilterOperator.LessThanOrEqual),
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<StockMovement>>> GetByMovementTypeAsync(PagedRequest pagedRequest, short movementType, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("MovementType", movementType),
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<StockMovement>>> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("ProductId", productId)
                      .WithFilter("WarehouseId", warehouseId),
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<decimal>> GetCurrentStockAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<StockMovementEntity>()
                .Where(StockMovementFields.ProductId == productId & StockMovementFields.WarehouseId == warehouseId)
                .OrderBy(StockMovementFields.CreatedAt.Descending())
                .Limit(1);
            
            var lastMovement = await adapter.FetchFirstAsync(query, cancellationToken);
            
            var currentStock = lastMovement?.NewStock ?? 0;
            Logger.LogInformation("Current stock for product {ProductId} in warehouse {WarehouseId}: {Stock}", productId, warehouseId, currentStock);
            
            return Result<decimal>.Success(currentStock);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting current stock for product: {ProductId}, warehouse: {WarehouseId}", productId, warehouseId);
            return Result<decimal>.Failure("An error occurred while getting current stock.");
        }
    }

    public async Task<Result<PagedResult<StockMovement>>> GetRecentMovementsAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => { },
            StockMovementFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }
}

