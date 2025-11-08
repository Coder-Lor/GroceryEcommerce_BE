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

public class SupplierRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<SupplierRepository> logger
) : BasePagedRepository<SupplierEntity, Supplier>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), ISupplierRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Name", typeof(string)),
            new SearchableField("ContactName", typeof(string)),
            new SearchableField("ContactEmail", typeof(string)),
            new SearchableField("ContactPhone", typeof(string)),
            new SearchableField("Address", typeof(string)),
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
            new FieldMapping { FieldName = "ContactName", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ContactEmail", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ContactPhone", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "Address", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", SupplierFields.Name },
            { "ContactName", SupplierFields.ContactName },
            { "ContactEmail", SupplierFields.ContactEmail },
            { "ContactPhone", SupplierFields.ContactPhone },
            { "Address", SupplierFields.Address },
            { "CreatedAt", SupplierFields.CreatedAt }
        };
    }

    protected override EntityQuery<SupplierEntity> ApplySearch(EntityQuery<SupplierEntity> query, string searchTerm)
    {
        return query.Where(
            SupplierFields.Name.Contains(searchTerm) |
            SupplierFields.ContactName.Contains(searchTerm) |
            SupplierFields.ContactEmail.Contains(searchTerm) |
            SupplierFields.ContactPhone.Contains(searchTerm) |
            SupplierFields.Address.Contains(searchTerm)
        );
    }

    protected override EntityQuery<SupplierEntity> ApplySorting(EntityQuery<SupplierEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "name" => SupplierFields.Name,
            "contactname" => SupplierFields.ContactName,
            "contactemail" => SupplierFields.ContactEmail,
            "createdat" => SupplierFields.CreatedAt,
            _ => SupplierFields.Name
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<SupplierEntity> ApplyDefaultSorting(EntityQuery<SupplierEntity> query)
    {
        return query.OrderBy(SupplierFields.Name.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return SupplierFields.SupplierId;
    }

    protected override object GetEntityId(SupplierEntity entity, EntityField2 primaryKeyField)
    {
        return entity.SupplierId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return SupplierFields.SupplierId.In(ids);
    }

    // Implementation of ISupplierRepository methods
    public async Task<Result<Supplier?>> GetByIdAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(SupplierFields.SupplierId, supplierId, "Supplier", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<Supplier?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(SupplierFields.Name, name, "Supplier_Name", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<Supplier?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(SupplierFields.ContactEmail, email, "Supplier_Email", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<Supplier>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<SupplierEntity>().OrderBy(SupplierFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var suppliers = Mapper.Map<List<Supplier>>(entities);
            
            Logger.LogInformation("Fetched {Count} suppliers", suppliers.Count);
            return Result<List<Supplier>>.Success(suppliers);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all suppliers");
            return Result<List<Supplier>>.Failure("An error occurred while fetching suppliers.");
        }
    }

    public async Task<Result<Supplier>> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<SupplierEntity>(supplier);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            var domainEntity = Mapper.Map<Supplier>(entity);
            await CacheService.RemoveByPatternAsync("Supplier*", cancellationToken);
            
            Logger.LogInformation("Supplier created: {SupplierId}", entity.SupplierId);
            return Result<Supplier>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating supplier");
            return Result<Supplier>.Failure("An error occurred while creating the supplier.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<SupplierEntity>(supplier);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Supplier*", cancellationToken);
            
            Logger.LogInformation("Supplier updated: {SupplierId}", entity.SupplierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating supplier: {SupplierId}", supplier.SupplierId);
            return Result<bool>.Failure("An error occurred while updating the supplier.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new SupplierEntity(supplierId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("Supplier*", cancellationToken);
            
            Logger.LogInformation("Supplier deleted: {SupplierId}", supplierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting supplier: {SupplierId}", supplierId);
            return Result<bool>.Failure("An error occurred while deleting the supplier.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(SupplierFields.Name, name, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(SupplierFields.SupplierId, supplierId, cancellationToken);
    }

    public async Task<Result<List<Supplier>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<SupplierEntity>()
                .Where(SupplierFields.Name.Contains(searchTerm))
                .OrderBy(SupplierFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var suppliers = Mapper.Map<List<Supplier>>(entities);
            
            Logger.LogInformation("Found {Count} suppliers matching search term: {SearchTerm}", suppliers.Count, searchTerm);
            return Result<List<Supplier>>.Success(suppliers);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching suppliers by name: {SearchTerm}", searchTerm);
            return Result<List<Supplier>>.Failure("An error occurred while searching suppliers.");
        }
    }

    public async Task<Result<List<Supplier>>> SearchByContactAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<SupplierEntity>()
                .Where(
                    SupplierFields.ContactName.Contains(searchTerm) |
                    SupplierFields.ContactEmail.Contains(searchTerm) |
                    SupplierFields.ContactPhone.Contains(searchTerm)
                )
                .OrderBy(SupplierFields.Name.Ascending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var suppliers = Mapper.Map<List<Supplier>>(entities);
            
            Logger.LogInformation("Found {Count} suppliers matching contact search term: {SearchTerm}", suppliers.Count, searchTerm);
            return Result<List<Supplier>>.Success(suppliers);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error searching suppliers by contact: {SearchTerm}", searchTerm);
            return Result<List<Supplier>>.Failure("An error occurred while searching suppliers.");
        }
    }

    public async Task<Result<bool>> IsSupplierInUseAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<PurchaseOrderEntity>()
                .Where(PurchaseOrderFields.SupplierId == supplierId)
                .Limit(1)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            
            Logger.LogInformation("Supplier {SupplierId} in use status: {InUse}", supplierId, count > 0);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if supplier is in use: {SupplierId}", supplierId);
            return Result<bool>.Failure("An error occurred while checking supplier usage.");
        }
    }

    public async Task<Result<int>> GetPurchaseOrderCountBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await CountByFieldAsync(PurchaseOrderFields.SupplierId, supplierId, cancellationToken);
    }
}

