using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;


namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Cart;

public class ShoppingCartItemRepository(
    DataAccessAdapter adapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ShoppingCartItemRepository> logger
): BasePagedRepository<ShoppingCartItemEntity, ShoppingCartItem>(adapter, unitOfWorkService, mapper, cacheService, logger), IShoppingCartItemRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("ProductVariantId", typeof(Guid)),
            new SearchableField("CartId", typeof(Guid)),
            new SearchableField("Quantity", typeof(int)),
            new SearchableField("UnitPrice", typeof(decimal)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("UpdatedAt", typeof(DateTime))
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
            new FieldMapping { FieldName = "CartId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CartItemId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductVariantId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Quantity", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UnitPrice", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["cartid"] = ShoppingCartItemFields.CartId,
            ["cartitemid"] = ShoppingCartItemFields.CartItemId,
            ["productid"] = ShoppingCartItemFields.ProductId,
            ["productvariantid"] = ShoppingCartItemFields.ProductVariantId,
            ["quantity"] = ShoppingCartItemFields.Quantity,
            ["unitprice"] = ShoppingCartItemFields.UnitPrice,
            ["createdat"] = ShoppingCartItemFields.CreatedAt,
            ["updatedat"] = ShoppingCartItemFields.UpdatedAt
        };
    }

    protected override EntityQuery<ShoppingCartItemEntity> ApplySearch(EntityQuery<ShoppingCartItemEntity> query, string searchTerm)
    {
        // Không áp dụng search dạng text cho GUID/number fields; trả về nguyên query
        return query;
    }

    protected override EntityQuery<ShoppingCartItemEntity> ApplySorting(EntityQuery<ShoppingCartItemEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<ShoppingCartItemEntity> ApplyDefaultSorting(EntityQuery<ShoppingCartItemEntity> query)
    {
        return query.OrderBy(ShoppingCartItemFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ShoppingCartItemFields.CartItemId;
    }

    protected override object GetEntityId(ShoppingCartItemEntity entity, EntityField2 primaryKeyField)
    {
        return entity.CartItemId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<ShoppingCartItem?>> GetByIdAsync(Guid cartItemId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(GetPrimaryKeyField()!, cartItemId, "ShoppingCartItem", TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task<Result<PagedResult<ShoppingCartItem>>> GetByCartIdAsync(PagedRequest request, Guid cartId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("CartId", cartId), cancellationToken: cancellationToken);
    }

    public Task<Result<ShoppingCartItem?>> GetByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        return GetByProductInternalAsync(cartId, productId, variantId, cancellationToken);
    }

    public Task<Result<ShoppingCartItem>> CreateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        return CreateInternalAsync(item, cancellationToken);
    }

    public Task<Result<bool>> UpdateAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        return UpdateInternalAsync(item, cancellationToken);
    }

    public Task<Result<bool>> DeleteAsync(Guid cartItemId, CancellationToken cancellationToken = default)
    {
        return DeleteInternalAsync(cartItemId, cancellationToken);
    }

    public Task<Result<bool>> ExistsAsync(Guid cartItemId, CancellationToken cancellationToken = default)
    {
        return ExistsByCountAsync(ShoppingCartItemFields.CartItemId, cartItemId, cancellationToken);
    }

    public Task<Result<bool>> UpdateQuantityAsync(Guid cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        return UpdateQuantityInternalAsync(cartItemId, quantity, cancellationToken);
    }

    public Task<Result<bool>> RemoveByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        return RemoveByProductInternalAsync(cartId, productId, variantId, cancellationToken);
    }

    public Task<Result<int>> GetItemCountByCartAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        return GetItemCountByCartInternalAsync(cartId, cancellationToken);
    }

    public Task<Result<decimal>> GetCartTotalAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        return GetCartTotalInternalAsync(cartId, cancellationToken);
    }

    public Task<Result<bool>> ClearCartAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        return ClearCartInternalAsync(cartId, cancellationToken);
    }

    private async Task<Result<ShoppingCartItem?>> GetByProductInternalAsync(Guid cartId, Guid productId, Guid? variantId, CancellationToken cancellationToken)
    {
        try
        {
            if (cartId == Guid.Empty || productId == Guid.Empty)
            {
                return Result<ShoppingCartItem?>.Failure("Invalid IDs");
            }
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>()
                .Where(ShoppingCartItemFields.CartId == cartId & ShoppingCartItemFields.ProductId == productId);
            if (variantId.HasValue)
            {
                query = query.Where(ShoppingCartItemFields.ProductVariantId == variantId.Value);
            }
            else
            {
                query = query.Where(ShoppingCartItemFields.ProductVariantId.IsNull());
            }
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null
                ? Result<ShoppingCartItem?>.Success(null)
                : Result<ShoppingCartItem?>.Success(Mapper.Map<ShoppingCartItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error GetByProductAsync");
            return Result<ShoppingCartItem?>.Failure("Error fetching item by product");
        }
    }

    private async Task<Result<ShoppingCartItem>> CreateInternalAsync(ShoppingCartItem item, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartItemEntity>(item);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                return Result<ShoppingCartItem>.Failure("Create cart item failed");
            }
            return Result<ShoppingCartItem>.Success(item);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating cart item");
            return Result<ShoppingCartItem>.Failure("Error creating cart item");
        }
    }

    private async Task<Result<bool>> UpdateInternalAsync(ShoppingCartItem item, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartItemEntity>(item);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update cart item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating cart item");
            return Result<bool>.Failure("Error updating cart item");
        }
    }

    private async Task<Result<bool>> DeleteInternalAsync(Guid cartItemId, CancellationToken cancellationToken)
    {
        try
        {
            if (cartItemId == Guid.Empty) return Result<bool>.Failure("Invalid cartItemId");
            var adapter = GetAdapter();
            var entity = new ShoppingCartItemEntity(cartItemId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete cart item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting cart item");
            return Result<bool>.Failure("Error deleting cart item");
        }
    }

    private async Task<Result<bool>> UpdateQuantityInternalAsync(Guid cartItemId, int quantity, CancellationToken cancellationToken)
    {
        try
        {
            if (cartItemId == Guid.Empty) return Result<bool>.Failure("Invalid cartItemId");
            var adapter = GetAdapter();
            var entity = new ShoppingCartItemEntity(cartItemId)
            {
                Quantity = quantity
            };
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update quantity failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating quantity");
            return Result<bool>.Failure("Error updating quantity");
        }
    }

    private async Task<Result<bool>> RemoveByProductInternalAsync(Guid cartId, Guid productId, Guid? variantId, CancellationToken cancellationToken)
    {
        try
        {
            if (cartId == Guid.Empty || productId == Guid.Empty)
                return Result<bool>.Failure("Invalid IDs");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(ShoppingCartItemFields.CartId == cartId);
            bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductId == productId);
            if (variantId.HasValue)
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId == variantId.Value);
            else
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId.IsNull());
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(ShoppingCartItemEntity), bucket, cancellationToken);
            return affected > 0 ? Result<bool>.Success(true) : Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing by product");
            return Result<bool>.Failure("Error removing by product");
        }
    }

    private async Task<Result<int>> GetItemCountByCartInternalAsync(Guid cartId, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>().Where(ShoppingCartItemFields.CartId == cartId).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting items in cart");
            return Result<int>.Failure("Error counting items in cart");
        }
    }

    private async Task<Result<decimal>> GetCartTotalInternalAsync(Guid cartId, CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>().Where(ShoppingCartItemFields.CartId == cartId);
            var items = await FetchEntitiesAsync(query, adapter, cancellationToken);
            decimal total = 0m;
            foreach (var e in items)
            {
                total += e.UnitPrice * e.Quantity;
            }
            return Result<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating cart total");
            return Result<decimal>.Failure("Error calculating cart total");
        }
    }

    private async Task<Result<bool>> ClearCartInternalAsync(Guid cartId, CancellationToken cancellationToken)
    {
        try
        {
            if (cartId == Guid.Empty) return Result<bool>.Failure("Invalid cartId");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(ShoppingCartItemFields.CartId == cartId);
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(ShoppingCartItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing cart items");
            return Result<bool>.Failure("Error clearing cart items");
        }
    }
}