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

public class WarehouseRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<WarehouseRepository> logger
) : BasePagedRepository<WarehouseEntity, Warehouse>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IWarehouseRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Name", typeof(string)),
            new SearchableField("Code", typeof(string)),
            new SearchableField("Address", typeof(string)),
            new SearchableField("City", typeof(string)),
            new SearchableField("State", typeof(string)),
            new SearchableField("Country", typeof(string)),
            new SearchableField("Phone", typeof(string)),
            new SearchableField("IsActive", typeof(bool)),
            new SearchableField("CreatedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "Name";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Code", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Address", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "City", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "State", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Country", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Phone", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "IsActive", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", WarehouseFields.Name },
            { "Code", WarehouseFields.Code },
            { "Address", WarehouseFields.Address },
            { "City", WarehouseFields.City },
            { "State", WarehouseFields.State },
            { "Country", WarehouseFields.Country },
            { "Phone", WarehouseFields.Phone },
            { "IsActive", WarehouseFields.IsActive },
            { "CreatedAt", WarehouseFields.CreatedAt }
        };
    }

    protected override EntityQuery<WarehouseEntity> ApplySearch(EntityQuery<WarehouseEntity> query, string searchTerm)
    {
        return query.Where(
            WarehouseFields.Name.Contains(searchTerm) |
            WarehouseFields.Code.Contains(searchTerm) |
            WarehouseFields.Address.Contains(searchTerm) |
            WarehouseFields.City.Contains(searchTerm) |
            WarehouseFields.State.Contains(searchTerm) |
            WarehouseFields.Country.Contains(searchTerm) |
            WarehouseFields.Phone.Contains(searchTerm)
        );
    }

    protected override EntityQuery<WarehouseEntity> ApplySorting(EntityQuery<WarehouseEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "name" => WarehouseFields.Name,
            "code" => WarehouseFields.Code,
            "city" => WarehouseFields.City,
            "state" => WarehouseFields.State,
            "country" => WarehouseFields.Country,
            "isactive" => WarehouseFields.IsActive,
            "createdat" => WarehouseFields.CreatedAt,
            _ => WarehouseFields.Name
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<WarehouseEntity> ApplyDefaultSorting(EntityQuery<WarehouseEntity> query)
    {
        return query.OrderBy(WarehouseFields.Name.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return WarehouseFields.WarehouseId;
    }

    protected override object GetEntityId(WarehouseEntity entity, EntityField2 primaryKeyField)
    {
        return entity.WarehouseId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return WarehouseFields.WarehouseId.In(ids);
    }

    // Implementation of IWarehouseRepository methods
    public async Task<Result<Warehouse?>> GetByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(WarehouseFields.WarehouseId, warehouseId, "Warehouse", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<Warehouse?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(WarehouseFields.Code, code, "Warehouse_Code", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<Warehouse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WarehouseEntity>().OrderBy(WarehouseFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var warehouses = Mapper.Map<List<Warehouse>>(entities);
            
            Logger.LogInformation("Fetched {Count} warehouses", warehouses.Count);
            return Result<List<Warehouse>>.Success(warehouses);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all warehouses");
            return Result<List<Warehouse>>.Failure("An error occurred while fetching warehouses.");
        }
    }

    public async Task<Result<Warehouse>> CreateAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WarehouseEntity>(warehouse);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            var domainEntity = Mapper.Map<Warehouse>(entity);
            await CacheService.RemoveByPatternAsync("Warehouse*", cancellationToken);
            
            Logger.LogInformation("Warehouse created: {WarehouseId}", entity.WarehouseId);
            return Result<Warehouse>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating warehouse");
            return Result<Warehouse>.Failure("An error occurred while creating the warehouse.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WarehouseEntity>(warehouse);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Warehouse*", cancellationToken);
            
            Logger.LogInformation("Warehouse updated: {WarehouseId}", entity.WarehouseId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating warehouse: {WarehouseId}", warehouse.WarehouseId);
            return Result<bool>.Failure("An error occurred while updating the warehouse.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new WarehouseEntity(warehouseId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("Warehouse*", cancellationToken);
            
            Logger.LogInformation("Warehouse deleted: {WarehouseId}", warehouseId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting warehouse: {WarehouseId}", warehouseId);
            return Result<bool>.Failure("An error occurred while deleting the warehouse.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(WarehouseFields.WarehouseId, warehouseId, cancellationToken);
    }

    public async Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(WarehouseFields.Code, code, cancellationToken);
    }

    public async Task<Result<PagedResult<Warehouse>>> GetActiveWarehousesAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => { req.WithFilter("IsActive", true); },
            WarehouseFields.Name.Name,
            SortDirection.Ascending,
            cancellationToken
        );
    }

    public async Task<Result<int>> GetProductCountByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<StockMovementEntity>()
                .Where(StockMovementFields.WarehouseId == warehouseId)
                .Select(() => StockMovementFields.ProductId.CountDistinct().As("ProductCount"));
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            Logger.LogInformation("Product count for warehouse {WarehouseId}: {Count}", warehouseId, count);
            
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting product count for warehouse: {WarehouseId}", warehouseId);
            return Result<int>.Failure("An error occurred while getting product count.");
        }
    }

    public async Task<Result<bool>> IsWarehouseInUseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            // Check if warehouse has purchase orders
            var poQuery = qf.Create<PurchaseOrderEntity>()
                .Where(PurchaseOrderFields.WarehouseId == warehouseId)
                .Limit(1)
                .Select(() => Functions.CountRow());
            
            var poCount = await adapter.FetchScalarAsync<int>(poQuery, cancellationToken);
            
            if (poCount > 0)
            {
                Logger.LogInformation("Warehouse {WarehouseId} is in use (has purchase orders)", warehouseId);
                return Result<bool>.Success(true);
            }
            
            // Check if warehouse has stock movements
            var smQuery = qf.Create<StockMovementEntity>()
                .Where(StockMovementFields.WarehouseId == warehouseId)
                .Limit(1)
                .Select(() => Functions.CountRow());
            
            var smCount = await adapter.FetchScalarAsync<int>(smQuery, cancellationToken);
            
            Logger.LogInformation("Warehouse {WarehouseId} in use status: {InUse}", warehouseId, smCount > 0);
            return Result<bool>.Success(smCount > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if warehouse is in use: {WarehouseId}", warehouseId);
            return Result<bool>.Failure("An error occurred while checking warehouse usage.");
        }
    }
}

