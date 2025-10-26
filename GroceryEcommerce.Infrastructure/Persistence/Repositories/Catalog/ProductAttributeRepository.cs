using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class ProductAttributeRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductAttributeRepository> logger
    ) : BasePagedRepository<ProductAttributeEntity, ProductAttribute>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductAttributeRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>()
        {
            new SearchableField("AttributeId", typeof(Guid)),
            new SearchableField("AttributeType", typeof(int)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("DisplayName", typeof(string)),
            new SearchableField("DisplayOrder", typeof(int)),
            new SearchableField("IsRequired", typeof(bool)),
            new SearchableField("Name", typeof(string))
        };
    }
    public override string GetDefaultSortField()
    {
        return "DisplayOrder";
    }
    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>()
        {
            new FieldMapping { FieldName = "AttributeId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AttributeType", FieldType = typeof(int), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DisplayName", FieldType = typeof(string), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DisplayOrder", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsRequired", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }
    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            ["attributeid"] = ProductAttributeFields.AttributeId,
            ["attributetype"] = ProductAttributeFields.AttributeType,
            ["createdat"] = ProductAttributeFields.CreatedAt,
            ["displayname"] = ProductAttributeFields.DisplayName,
            ["displayorder"] = ProductAttributeFields.DisplayOrder,
            ["isrequired"] = ProductAttributeFields.IsRequired,
            ["name"] = ProductAttributeFields.Name 
        };
    }
    protected override EntityQuery<ProductAttributeEntity> ApplySearch(EntityQuery<ProductAttributeEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(
            ProductAttributeFields.DisplayName.Contains(searchTerm) |
            ProductAttributeFields.Name.Contains(searchTerm)
        );
    }
    
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "attributeid" => ProductAttributeFields.AttributeId,
            "attributetype" => ProductAttributeFields.AttributeType,
            "createdat" => ProductAttributeFields.CreatedAt,
            "displayname" => ProductAttributeFields.DisplayName,
            "displayorder" => ProductAttributeFields.DisplayOrder,
            "isrequired" => ProductAttributeFields.IsRequired,
            _ => ProductAttributeFields.Name
        };
    }

    protected override EntityQuery<ProductAttributeEntity> ApplySorting(EntityQuery<ProductAttributeEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductAttributeEntity> ApplyDefaultSorting(EntityQuery<ProductAttributeEntity> query)
    {
        return query.OrderBy(ProductAttributeFields.DisplayOrder.Ascending());   
    }

    protected override async Task<IList<ProductAttributeEntity>> FetchEntitiesAsync(EntityQuery<ProductAttributeEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductAttributeEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductAttributeFields.AttributeId;
    }

    protected override object GetEntityId(ProductAttributeEntity entity, EntityField2 primaryKeyField)
    {
        return entity.AttributeId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public async Task<Result<ProductAttribute?>> GetByIdAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        if (attributeId == Guid.Empty) {
            logger.LogWarning("Attribute id is required");
            return Result<ProductAttribute?>.Failure("Invalid attribute ID.");
        }
        return await GetSingleAsync(ProductAttributeFields.AttributeId, attributeId, "ProductAttribute", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<ProductAttribute?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) {
            logger.LogWarning("Attribute name is required");
            return Result<ProductAttribute?>.Failure("Invalid attribute name.");
        }
        return await GetSingleAsync(ProductAttributeFields.Name, name, "ProductAttribute", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<List<ProductAttribute>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try {
            var cacheKey = "All_ProductAttributes";
            var cachedAttributes = await CacheService.GetAsync<List<ProductAttribute>>(cacheKey, cancellationToken);
            if (cachedAttributes != null) {
                logger.LogInformation("Attributes fetched from cache");
                return Result<List<ProductAttribute>>.Success(cachedAttributes);
            }
            var adapter = GetAdapter();
            var query = @"SELECT * FROM catalog.product_attributes ORDER BY display_order ASC;";
            var entities = await adapter.FetchQueryAsync<ProductAttributeEntity>(query, cancellationToken);

            var attributes = Mapper.Map<List<ProductAttribute>>(entities);
            await CacheService.SetAsync(cacheKey, attributes, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Attributes fetched from database and cached");
            
            return Result<List<ProductAttribute>>.Success(attributes);
        }
        catch (Exception ex) { 
            logger.LogError(ex, "Error getting attribute by id");
            return Result<List<ProductAttribute>>.Failure("An error occurred while retrieving the attributes.");
        }
    }

    public async Task<Result<ProductAttribute>> CreateAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductAttributeEntity>(attribute);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductAttributes", cancellationToken);
                await CacheService.RemoveAsync($"Attribute_{attribute.AttributeId}", cancellationToken);
                logger.LogInformation("Attribute created: {Name}", attribute.Name);
                return Result<ProductAttribute>.Success(Mapper.Map<ProductAttribute>(entity));
            }
            logger.LogWarning("Attribute not created: {Name}", attribute.Name);
            return Result<ProductAttribute>.Failure("Attribute not created.");
        }
        catch (Exception ex) { 
            logger.LogError(ex, "Error creating attribute: {Name}", attribute.Name);
            return Result<ProductAttribute>.Failure("An error occurred while creating attribute.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductAttribute attribute, CancellationToken cancellationToken = default)
    {
        try
        {
            var qf = new QueryFactory();
            var query = qf.ProductAttribute.Where(ProductAttributeFields.AttributeId == attribute.AttributeId);
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity is null) {
                logger.LogWarning("Attribute not found for update: {AttributeId}", attribute.AttributeId);
                return Result<bool>.Failure("Attribute not found.");
            }
            entity = Mapper.Map(attribute, entity);
            entity.IsNew = false;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductAttributes", cancellationToken);
                await CacheService.RemoveAsync($"Attribute_{attribute.AttributeId}", cancellationToken);
                logger.LogInformation("Attribute updated: {Name}", attribute.Name);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Attribute not updated: {Name}", attribute.Name);
            return Result<bool>.Failure("Attribute not updated.");
        }
        catch (Exception ex) { 
            logger.LogError(ex, "Error updating attribute: {Name}", attribute.Name);
            return Result<bool>.Failure("An error occurred while updating attribute.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductAttributeEntity(attributeId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync("All_ProductAttributes", cancellationToken);
                await CacheService.RemoveAsync($"Attribute_{attributeId}", cancellationToken);
                logger.LogInformation("Attribute deleted: {AttributeId}", attributeId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Attribute not deleted: {AttributeId}", attributeId);
            return Result<bool>.Failure("Attribute not deleted.");
        }
        catch (Exception ex) { 
            logger.LogError(ex, "Error deleting attribute: {AttributeId}", attributeId);
            return Result<bool>.Failure("An error occurred while deleting attribute.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name)) {
            logger.LogWarning("Attribute name is required");
            return Result<bool>.Failure("Invalid attribute name.");
        }
        return await ExistsByCountAsync(ProductAttributeFields.Name, name, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        if (attributeId == Guid.Empty) {
            logger.LogWarning("Attribute id is required");
            return Result<bool>.Failure("Invalid attribute ID.");
        }
        return await ExistsByCountAsync(ProductAttributeFields.AttributeId, attributeId, cancellationToken);
    }

    public async Task<Result<PagedResult<ProductAttribute>>> GetRequiredAttributesAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("IsRequired", true), "DisplayOrder", SortDirection.Ascending, cancellationToken);


    public async Task<Result<PagedResult<ProductAttribute>>> GetByTypeAsync(PagedRequest request, short attributeType, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("AttributeType", attributeType), "DisplayOrder", SortDirection.Ascending, cancellationToken);

    public async Task<Result<bool>> IsAttributeInUseAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        if (attributeId == Guid.Empty) {
            logger.LogWarning("Attribute id is required");
            return Result<bool>.Failure("Invalid attribute ID.");
        }
        return await ExistsByCountAsync(ProductAttributeValueFields.AttributeId, attributeId, cancellationToken);
    }
}