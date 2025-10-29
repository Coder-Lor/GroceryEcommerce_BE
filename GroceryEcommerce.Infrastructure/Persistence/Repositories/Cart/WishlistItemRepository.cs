using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Cart;

public class WishlistItemRepository(
    DataAccessAdapter adapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<WishlistItemRepository> logger
): BasePagedRepository<WishlistItemEntity, WishlistItem>(adapter, unitOfWorkService, mapper, cacheService, logger), IWishlistItemRepository 
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("WishlistId", typeof(Guid)),
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("ProductVariantId", typeof(Guid)),
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
            new FieldMapping { FieldName = "WishlistItemId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "WishlistId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductVariantId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["wishlistitemid"] = WishlistItemFields.WishlistItemId,
            ["wishlistid"] = WishlistItemFields.WishlistId,
            ["productid"] = WishlistItemFields.ProductId,
            ["productvariantid"] = WishlistItemFields.ProductVariantId,
            ["createdat"] = WishlistItemFields.CreatedAt
        };
    }

    protected override EntityQuery<WishlistItemEntity> ApplySearch(EntityQuery<WishlistItemEntity> query, string searchTerm)
    {
        return query; // không search text
    }

    protected override EntityQuery<WishlistItemEntity> ApplySorting(EntityQuery<WishlistItemEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<WishlistItemEntity> ApplyDefaultSorting(EntityQuery<WishlistItemEntity> query)
    {
        return query.OrderBy(WishlistItemFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return WishlistItemFields.WishlistItemId;
    }

    protected override object GetEntityId(WishlistItemEntity entity, EntityField2 primaryKeyField)
    {
        return entity.WishlistItemId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<WishlistItem?>> GetByIdAsync(Guid wishlistItemId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(WishlistItemFields.WishlistItemId, wishlistItemId, "WishlistItem", TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task<Result<PagedResult<WishlistItem>>> GetByWishlistIdAsync(PagedRequest request, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("WishlistId", wishlistId), cancellationToken: cancellationToken);
    }

    public Task<Result<WishlistItem?>> GetByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        return GetByProductInternalAsync(wishlistId, productId, variantId, cancellationToken);
    }

    public async Task<Result<WishlistItem>> CreateAsync(WishlistItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistItemEntity>(item);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<WishlistItem>.Success(item) : Result<WishlistItem>.Failure("Create wishlist item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating wishlist item");
            return Result<WishlistItem>.Failure("Error creating wishlist item");
        }
    }

    public async Task<Result<bool>> UpdateAsync(WishlistItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistItemEntity>(item);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update wishlist item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating wishlist item");
            return Result<bool>.Failure("Error updating wishlist item");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid wishlistItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistItemId == Guid.Empty) return Result<bool>.Failure("Invalid wishlistItemId");
            var adapter = GetAdapter();
            var entity = new WishlistItemEntity(wishlistItemId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete wishlist item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting wishlist item");
            return Result<bool>.Failure("Error deleting wishlist item");
        }
    }

    public Task<Result<bool>> ExistsAsync(Guid wishlistItemId, CancellationToken cancellationToken = default)
    {
        return ExistsByCountAsync(WishlistItemFields.WishlistItemId, wishlistItemId, cancellationToken);
    }

    public async Task<Result<bool>> RemoveByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty || productId == Guid.Empty)
                return Result<bool>.Failure("Invalid IDs");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(WishlistItemFields.WishlistId == wishlistId);
            bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductId == productId);
            if (variantId.HasValue)
                bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductVariantId == variantId.Value);
            else
                bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductVariantId.IsNull());
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(WishlistItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing wishlist item by product");
            return Result<bool>.Failure("Error removing wishlist item by product");
        }
    }

    public async Task<Result<int>> GetItemCountByWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistItemEntity>().Where(WishlistItemFields.WishlistId == wishlistId).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting wishlist items");
            return Result<int>.Failure("Error counting wishlist items");
        }
    }

    public async Task<Result<bool>> ClearWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty) return Result<bool>.Failure("Invalid wishlistId");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(WishlistItemFields.WishlistId == wishlistId);
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(WishlistItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing wishlist items");
            return Result<bool>.Failure("Error clearing wishlist items");
        }
    }

    public Task<Result<PagedResult<WishlistItem>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("UserId", userId), cancellationToken: cancellationToken);
    }

    private async Task<Result<WishlistItem?>> GetByProductInternalAsync(Guid wishlistId, Guid productId, Guid? variantId, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistItemEntity>()
                .Where(WishlistItemFields.WishlistId == wishlistId & WishlistItemFields.ProductId == productId);
            if (variantId.HasValue)
                query = query.Where(WishlistItemFields.ProductVariantId == variantId.Value);
            else
                query = query.Where(WishlistItemFields.ProductVariantId.IsNull());
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null
                ? Result<WishlistItem?>.Success(null)
                : Result<WishlistItem?>.Success(Mapper.Map<WishlistItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error GetByProduct wishlist item");
            return Result<WishlistItem?>.Failure("Error fetching wishlist item by product");
        }
    }
}