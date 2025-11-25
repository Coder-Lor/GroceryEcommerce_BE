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

public class ProductVariantRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductVariantRepository> logger
) : BasePagedRepository<ProductVariantEntity, ProductVariant>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductVariantRepository
{

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "createdat" => ProductVariantFields.CreatedAt,
            "discountprice" => ProductVariantFields.DiscountPrice,
            "imageurl" => ProductVariantFields.ImageUrl,
            "name" => ProductVariantFields.Name,
            "price" => ProductVariantFields.Price,
            "productid" => ProductVariantFields.ProductId,
            "sku" => ProductVariantFields.Sku,
            "status" => ProductVariantFields.Status,
            "stockquantity" => ProductVariantFields.StockQuantity,
            "updatedat" => ProductVariantFields.UpdatedAt,
            "variantid" => ProductVariantFields.VariantId,
            "weight" => ProductVariantFields.Weight,
            _ => ProductVariantFields.VariantId
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("DiscountPrice", typeof(decimal)),
            new SearchableField("ImageUrl", typeof(string)),
            new SearchableField("Name", typeof(string)),
            new SearchableField("Price", typeof(decimal)),
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("Sku", typeof(string)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("StockQuantity", typeof(int)),
            new SearchableField("UpdatedAt", typeof(DateTime)),
            new SearchableField("VariantId", typeof(Guid)),
            new SearchableField("Weight", typeof(decimal)),
        };
    }
    public override string GetDefaultSortField()
    {
        return "VariantId";
    }
    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DiscountPrice", FieldType = typeof(decimal), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ImageUrl", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Price", FieldType = typeof(decimal), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Sku", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "StockQuantity", FieldType = typeof(int), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "VariantId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Weight", FieldType = typeof(decimal), IsSearchable = true, IsSortable = true, IsFilterable = true },
        };
    }   
    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase) {
            ["createdat"] = ProductVariantFields.CreatedAt,
            ["discountprice"] = ProductVariantFields.DiscountPrice,
            ["imageurl"] = ProductVariantFields.ImageUrl,
            ["name"] = ProductVariantFields.Name,
            ["price"] = ProductVariantFields.Price,
            ["productid"] = ProductVariantFields.ProductId,
            ["sku"] = ProductVariantFields.Sku,
            ["status"] = ProductVariantFields.Status,
            ["stockquantity"] = ProductVariantFields.StockQuantity,
            ["updatedat"] = ProductVariantFields.UpdatedAt,
            ["variantid"] = ProductVariantFields.VariantId,
            ["weight"] = ProductVariantFields.Weight,
        };
    }
    protected override EntityQuery<ProductVariantEntity> ApplySearch(EntityQuery<ProductVariantEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ProductVariantFields.Name,
            ProductVariantFields.Sku,
            ProductVariantFields.ProductId,
            ProductVariantFields.Status,
            ProductVariantFields.StockQuantity);

        return query.Where(predicate);
    }

    protected override EntityQuery<ProductVariantEntity> ApplySorting(EntityQuery<ProductVariantEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductVariantEntity> ApplyDefaultSorting(EntityQuery<ProductVariantEntity> query)
    {
        return query.OrderBy(ProductVariantFields.VariantId.Ascending());
    }

    protected override async Task<IList<ProductVariantEntity>> FetchEntitiesAsync(EntityQuery<ProductVariantEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductVariantEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductVariantFields.VariantId;
    }

    protected override object GetEntityId(ProductVariantEntity entity, EntityField2 primaryKeyField)
    {
        return entity.VariantId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<ProductVariant?>> GetByIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        if (variantId == Guid.Empty)
        {
            logger.LogWarning("Variant id is required");
            return Task.FromResult(Result<ProductVariant?>.Failure("Invalid variant ID."));
        }
        return GetSingleAsync(ProductVariantFields.VariantId, variantId, "ProductVariant", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductVariant>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("ProductId", productId), "ProductId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductVariant>>> GetBySkuAsync(PagedRequest request, string sku, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("Sku", sku), "Sku", SortDirection.Ascending, cancellationToken);

    public async Task<Result<bool>> CreateAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductVariantEntity>(variant);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductVariants", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariant_{variant.VariantId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_ByProduct_{variant.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_BySku_{variant.Sku}", cancellationToken);
                logger.LogInformation("Product variant created: {Variant}", variant);

                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product variant not created: {Variant}", variant);
            return Result<bool>.Failure("Product variant not created.");
        }
        catch (Exception ex){
            logger.LogError(ex, "Error creating product variant");
            return Result<bool>.Failure("An error occurred while creating product variant.", ex.Message);
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        try {
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(
                new QueryFactory().ProductVariant.Where(ProductVariantFields.VariantId == variant.VariantId),
                cancellationToken
            );
            if (entity is null) {
                logger.LogWarning("Product variant not found for update: {VariantId}", variant.VariantId);
                return Result<bool>.Failure("Product variant not found.");
            }
            entity = Mapper.Map(variant, entity);
            entity.IsNew = false;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);

            if (saved) {
                await CacheService.RemoveAsync("All_ProductVariants", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariant_{variant.VariantId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_ByProduct_{variant.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_BySku_{variant.Sku}", cancellationToken);
                logger.LogInformation("Product variant updated: {Variant}", variant);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product variant not updated: {Variant}", variant);
            return Result<bool>.Failure("Product variant not updated.");
        }
        catch (Exception ex){
            logger.LogError(ex, "Error updating product variant");
            return Result<bool>.Failure("An error occurred while updating product variant.", ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        try {
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(
                new QueryFactory().ProductVariant.Where(ProductVariantFields.VariantId == variantId),
                cancellationToken
            );
            if (entity is null) {
                logger.LogWarning("Product variant not found for delete: {VariantId}", variantId);
                return Result<bool>.Failure("Product variant not found.");
            }
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync("All_ProductVariants", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariant_{variantId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_ByProduct_{entity.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_BySku_{entity.Sku}", cancellationToken);
                logger.LogInformation("Product variant deleted: {VariantId}", variantId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product variant not deleted: {VariantId}", variantId);
            return Result<bool>.Failure("Product variant not deleted.");
        }
        catch (Exception ex){
            logger.LogError(ex, "Error deleting product variant");
            return Result<bool>.Failure("An error occurred while deleting product variant.", ex.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        if (variantId == Guid.Empty) {
            logger.LogWarning("Variant id is required");
            return Result<bool>.Failure("Invalid variant ID.");
        }
        return await ExistsByCountAsync(ProductVariantFields.VariantId, variantId, cancellationToken);
    }

    public async Task<Result<ProductVariant?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku)) {
            logger.LogWarning("Sku is required");
            return Result<ProductVariant?>.Failure("Invalid sku.");
        }
        return await GetSingleAsync(ProductVariantFields.Sku, sku, "ProductVariant", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductVariant>>> GetVariantsWithStockAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("ProductId", productId), "ProductId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<bool>> UpdateStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken = default)
    {
        try {
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(
                new QueryFactory().ProductVariant.Where(ProductVariantFields.VariantId == variantId),
                cancellationToken
            );
            if (entity is null) {
                logger.LogWarning("Product variant not found for update stock: {VariantId}", variantId);
                return Result<bool>.Failure("Product variant not found.");
            }
            entity.StockQuantity = quantity;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductVariants", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariant_{variantId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_ByProduct_{entity.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductVariants_BySku_{entity.Sku}", cancellationToken);
                logger.LogInformation("Product variant updated stock: {VariantId}", variantId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product variant not updated stock: {VariantId}", variantId);
            return Result<bool>.Failure("Product variant not updated stock.");
        }
        catch (Exception ex){
            logger.LogError(ex, "Error updating product variant stock");
            return Result<bool>.Failure("An error occurred while updating product variant stock.", ex.Message);
        }
    }

    public async Task<Result<int>> GetTotalStockByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try {
            var query = new QueryFactory().ProductVariant
                .Where(ProductVariantFields.ProductId == productId)
                .Select(ProductVariantFields.StockQuantity.Sum());
            
            var adapter = GetAdapter();
            var totalStock = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            logger.LogInformation("Total stock fetched for product: {ProductId}, Total Stock: {TotalStock}", productId, totalStock);
            return Result<int>.Success(totalStock);
        }
        catch (Exception ex){
            logger.LogError(ex, "Error getting total stock by product");
            return Result<int>.Failure("An error occurred while getting total stock by product.", ex.Message);
        }
    }

    public async Task<Result<PagedResult<ProductVariant>>> GetLowStockVariantsAsync(PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("StockQuantity", threshold, FilterOperator.LessThanOrEqual), "StockQuantity", SortDirection.Ascending, cancellationToken);
}