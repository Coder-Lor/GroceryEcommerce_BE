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

public class ShipmentCarrierRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ShipmentCarrierRepository> logger
) : BasePagedRepository<ShipmentCarrierEntity, ShipmentCarrier>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IShipmentCarrierRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Name", typeof(string)),
            new SearchableField("Code", typeof(string)),
            new SearchableField("Website", typeof(string)),
            new SearchableField("Phone", typeof(string)),
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
            new FieldMapping { FieldName = "Website", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "Phone", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "name", ShipmentCarrierFields.Name },
            { "code", ShipmentCarrierFields.Code },
            { "website", ShipmentCarrierFields.Website },
            { "phone", ShipmentCarrierFields.Phone },
            { "createdat", ShipmentCarrierFields.CreatedAt }
        };
    }

    protected override EntityQuery<ShipmentCarrierEntity> ApplySearch(EntityQuery<ShipmentCarrierEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ShipmentCarrierFields.Name,
            ShipmentCarrierFields.Code,
            ShipmentCarrierFields.Website,
            ShipmentCarrierFields.Phone);

        return query.Where(predicate);
    }

    protected override EntityQuery<ShipmentCarrierEntity> ApplySorting(EntityQuery<ShipmentCarrierEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "name" => ShipmentCarrierFields.Name,
            "code" => ShipmentCarrierFields.Code,
            "website" => ShipmentCarrierFields.Website,
            "phone" => ShipmentCarrierFields.Phone,
            "createdat" => ShipmentCarrierFields.CreatedAt,
            _ => ShipmentCarrierFields.Name
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<ShipmentCarrierEntity> ApplyDefaultSorting(EntityQuery<ShipmentCarrierEntity> query)
    {
        return query.OrderBy(ShipmentCarrierFields.Name.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ShipmentCarrierFields.CarrierId;
    }

    protected override object GetEntityId(ShipmentCarrierEntity entity, EntityField2 primaryKeyField)
    {
        return entity.CarrierId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return ShipmentCarrierFields.CarrierId.In(ids);
    }

    public async Task<Result<ShipmentCarrier?>> GetByIdAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(ShipmentCarrierFields.CarrierId, carrierId, "ShipmentCarrier", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<ShipmentCarrier?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(ShipmentCarrierFields.Name, name, "ShipmentCarrier_Name", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<ShipmentCarrier?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(ShipmentCarrierFields.Code, code, "ShipmentCarrier_Code", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<ShipmentCarrier>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentCarrierEntity>()
                .OrderBy(ShipmentCarrierFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<ShipmentCarrier>>(entities);
            
            return Result<List<ShipmentCarrier>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching all shipment carriers");
            return Result<List<ShipmentCarrier>>.Failure("An error occurred while fetching shipment carriers.");
        }
    }

    public async Task<Result<bool>> CreateAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentCarrierEntity>(carrier);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier created: {CarrierId}", entity.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating shipment carrier");
            return Result<bool>.Failure("An error occurred while creating shipment carrier.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentCarrierEntity>(carrier);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier updated: {CarrierId}", entity.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating shipment carrier: {CarrierId}", carrier.CarrierId);
            return Result<bool>.Failure("An error occurred while updating shipment carrier.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShipmentCarrierEntity(carrierId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier deleted: {CarrierId}", carrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting shipment carrier: {CarrierId}", carrierId);
            return Result<bool>.Failure("An error occurred while deleting shipment carrier.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentCarrierEntity>()
                .Where(ShipmentCarrierFields.Name == name)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking carrier existence by name: {Name}", name);
            return Result<bool>.Failure("An error occurred while checking carrier existence.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(ShipmentCarrierFields.CarrierId, carrierId, cancellationToken);
    }

    public async Task<Result<List<ShipmentCarrier>>> GetActiveCarriersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentCarrierEntity>()
                .OrderBy(ShipmentCarrierFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<ShipmentCarrier>>(entities);
            
            return Result<List<ShipmentCarrier>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching active carriers");
            return Result<List<ShipmentCarrier>>.Failure("An error occurred while fetching active carriers.");
        }
    }

    public async Task<Result<bool>> IsCarrierInUseAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderShipmentEntity>()
                .Where(OrderShipmentFields.CarrierId == carrierId)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if carrier is in use: {CarrierId}", carrierId);
            return Result<bool>.Failure("An error occurred while checking carrier usage.");
        }
    }

    public async Task<Result<int>> GetShipmentCountByCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderShipmentEntity>()
                .Where(OrderShipmentFields.CarrierId == carrierId)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting shipments for carrier: {CarrierId}", carrierId);
            return Result<int>.Failure("An error occurred while counting shipments.");
        }
    }
}

