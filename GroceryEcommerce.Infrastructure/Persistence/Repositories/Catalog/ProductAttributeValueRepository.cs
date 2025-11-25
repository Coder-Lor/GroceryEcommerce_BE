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

public class ProductAttributeValueRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductAttributeValueRepository> logger
) : BasePagedRepository<ProductAttributeValueEntity, ProductAttributeValue>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductAttributeValueRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>()
        {
            new SearchableField("ValueId", typeof(Guid)),
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("AttributeId", typeof(Guid)),
            new SearchableField("Value", typeof(string))
        };
    }

    public override string GetDefaultSortField()
    {
        return "ValueId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>()
        {
            new FieldMapping { FieldName = "ValueId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AttributeId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Value", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>()
        {
            ["valueid"] = ProductAttributeValueFields.ValueId,
            ["productid"] = ProductAttributeValueFields.ProductId,
            ["attributeid"] = ProductAttributeValueFields.AttributeId,
            ["value"] = ProductAttributeValueFields.Value
        };
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplySearch(EntityQuery<ProductAttributeValueEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ProductAttributeValueFields.Value,
            ProductAttributeValueFields.ProductId,
            ProductAttributeValueFields.AttributeId);

        return query.Where(predicate);
    }

        private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "valueid" => ProductAttributeValueFields.ValueId,
            "productid" => ProductAttributeValueFields.ProductId,
            "attributeid" => ProductAttributeValueFields.AttributeId,
            "value" => ProductAttributeValueFields.Value,
            _ => ProductAttributeValueFields.ValueId
        };
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplySorting(EntityQuery<ProductAttributeValueEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplyDefaultSorting(EntityQuery<ProductAttributeValueEntity> query)
    {
        return query.OrderBy(ProductAttributeValueFields.ValueId.Ascending());
    }

    protected override async Task<IList<ProductAttributeValueEntity>> FetchEntitiesAsync(EntityQuery<ProductAttributeValueEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductAttributeValueEntity>();
        await scopedAdapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductAttributeValueFields.ValueId;
    }

    protected override object GetEntityId(ProductAttributeValueEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ValueId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public async Task<Result<ProductAttributeValue?>> GetByIdAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        if (valueId == Guid.Empty) {
            logger.LogWarning("Value id is required");
            return Result<ProductAttributeValue?>.Failure("Invalid value ID.");
        }
        return await GetSingleAsync(ProductAttributeValueFields.ValueId, valueId, "ProductAttributeValue", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductAttributeValue>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("ProductId", productId), "ValueId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductAttributeValue>>> GetByAttributeIdAsync(PagedRequest request, Guid attributeId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("AttributeId", attributeId), "ValueId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<ProductAttributeValue>> CreateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductAttributeValueEntity>(value);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var created = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (created) {
                await CacheService.RemoveAsync($"ProductAttributeValue_{value.ValueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProduct_{value.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByAttribute_{value.AttributeId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByValue_{value.Value}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProductAndAttribute_{value.ProductId}_{value.AttributeId}", cancellationToken);
                logger.LogInformation("Product attribute value created: {Value}", value);
                return Result<ProductAttributeValue>.Success(Mapper.Map<ProductAttributeValue>(entity));
            }
            logger.LogWarning("Product attribute value not created: {Value}", value);
            return Result<ProductAttributeValue>.Failure("Product attribute value not created.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error creating product attribute value: {Value}", value);
            return Result<ProductAttributeValue>.Failure("An error occurred while creating product attribute value.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductAttributeValueEntity>(value);
            entity.IsNew = false;
            var adapter = GetAdapter();
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated) {
                await CacheService.RemoveAsync($"ProductAttributeValue_{value.ValueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProduct_{value.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByAttribute_{value.AttributeId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByValue_{value.Value}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProductAndAttribute_{value.ProductId}_{value.AttributeId}", cancellationToken);
                logger.LogInformation("Product attribute value updated: {Value}", value);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product attribute value not updated: {Value}", value);
            return Result<bool>.Failure("Product attribute value not updated.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating product attribute value: {Value}", value);
            return Result<bool>.Failure("An error occurred while updating product attribute value.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductAttributeValueEntity(valueId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync($"ProductAttributeValue_{valueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProduct_{valueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByAttribute_{valueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByValue_{valueId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductAttributeValues_ByProductAndAttribute_{valueId}", cancellationToken);
                logger.LogInformation("Product attribute value deleted: {ValueId}", valueId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product attribute value not deleted: {ValueId}", valueId);
            return Result<bool>.Failure("Product attribute value not deleted.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product attribute value: {ValueId}", valueId);
            return Result<bool>.Failure("An error occurred while deleting product attribute value.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        if (valueId == Guid.Empty) {
            logger.LogWarning("Value id is required");
            return Result<bool>.Failure("Invalid value ID.");
        }
        return await ExistsByCountAsync(ProductAttributeValueFields.ValueId, valueId, cancellationToken);
    }

    public async Task<Result<ProductAttributeValue?>> GetByProductAndAttributeAsync(Guid productId, Guid attributeId, CancellationToken cancellationToken = default)
    {
        try {
            var cached = await CacheService.GetAsync<ProductAttributeValue>($"ProductAttributeValue_{productId}_{attributeId}", cancellationToken);
            if (cached != null) {
                logger.LogInformation("Product attribute value found in cache: {ProductId}, {AttributeId}", productId, attributeId);
                return Result<ProductAttributeValue?>.Success(cached);
            }
            var query = new QueryFactory().
                ProductAttributeValue.Where(
                    ProductAttributeValueFields.ProductId == productId & 
                    ProductAttributeValueFields.AttributeId == attributeId
                );
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) {
                logger.LogWarning("Product attribute value not found: {ProductId}, {AttributeId}", productId, attributeId);
                return Result<ProductAttributeValue?>.Failure("Product attribute value not found.");
            }
            var value = Mapper.Map<ProductAttributeValue>(entity);
            await CacheService.SetAsync($"ProductAttributeValue_{productId}_{attributeId}", value, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product attribute value found: {ProductId}, {AttributeId}", productId, attributeId);
            return Result<ProductAttributeValue?>.Success(value);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error getting product attribute value by product and attribute: {ProductId}, {AttributeId}", productId, attributeId);
            return Result<ProductAttributeValue?>.Failure("An error occurred while getting product attribute value by product and attribute.");
        }
    }

    public async Task<Result<bool>> DeleteByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try {
            var query = new QueryFactory().
                ProductAttributeValue.Where(
                    ProductAttributeValueFields.ProductId == productId
                );

            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) {
                logger.LogWarning("Product attribute values not found by product: {ProductId}", productId);
                return Result<bool>.Failure("Product attribute values not found.");
            }
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                logger.LogInformation("Product attribute values deleted by product: {ProductId}", productId);
                return Result<bool>.Success(true);
            }

            logger.LogWarning("Product attribute values not deleted by product: {ProductId}", productId);
            return Result<bool>.Failure("Product attribute values not deleted by product.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product attribute value by product: {ProductId}", productId);
            return Result<bool>.Failure("An error occurred while deleting product attribute value by product.");
        }
    }

    public async Task<Result<bool>> DeleteByAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        try {
            var query = new QueryFactory().
                ProductAttributeValue.Where(
                    ProductAttributeValueFields.AttributeId == attributeId
                );
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) {
                logger.LogWarning("Product attribute values not found by attribute: {AttributeId}", attributeId);
                return Result<bool>.Failure("Product attribute values not found.");
            }
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                logger.LogInformation("Product attribute values deleted by attribute: {AttributeId}", attributeId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product attribute values not deleted by attribute: {AttributeId}", attributeId);
            return Result<bool>.Failure("Product attribute values not deleted by attribute.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product attribute value by attribute: {AttributeId}", attributeId);
            return Result<bool>.Failure("An error occurred while deleting product attribute value by attribute.");
        }
    }

    public async Task<Result<PagedResult<ProductAttributeValue>>> GetByValueAsync(PagedRequest request, string value, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Value", value), "ValueId", SortDirection.Ascending, cancellationToken);
}