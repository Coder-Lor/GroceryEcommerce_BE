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

public class CartRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    IUnitOfWorkService unitOfWorkService,
    ICacheService cacheService,
    ILogger<CartRepository> logger): BasePagedRepository<ShoppingCartEntity, ShoppingCart>(adapter, unitOfWorkService, mapper, cacheService, logger), ICartRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("CartId", typeof(Guid)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("SessionId", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("UpdatedAt", typeof(DateTime))
        };
    }

    public override string? GetDefaultSortField()
    {
        return "CreatedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "CartId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "SessionId", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["cartid"] = ShoppingCartFields.CartId,
            ["userid"] = ShoppingCartFields.UserId,
            ["sessionid"] = ShoppingCartFields.SessionId,
            ["createdat"] = ShoppingCartFields.CreatedAt,
            ["updatedat"] = ShoppingCartFields.UpdatedAt
        };
    }

    protected override EntityQuery<ShoppingCartEntity> ApplySearch(EntityQuery<ShoppingCartEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        var term = searchTerm.Trim().ToLower();
        return query.Where(ShoppingCartFields.SessionId.Contains(term));
    }

    protected override EntityQuery<ShoppingCartEntity> ApplySorting(EntityQuery<ShoppingCartEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<ShoppingCartEntity> ApplyDefaultSorting(EntityQuery<ShoppingCartEntity> query)
    {
        return query.OrderBy(ShoppingCartFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ShoppingCartFields.CartId;
    }

    protected override object GetEntityId(ShoppingCartEntity entity, EntityField2 primaryKeyField)
    {
        return entity.CartId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    // Shopping Cart operations
    public async Task<Result<ShoppingCart?>> GetShoppingCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<ShoppingCart?>.Failure("Invalid userId");
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartEntity>()
                .Where(ShoppingCartFields.UserId == userId)
                .WithPath(ShoppingCartEntity.PrefetchPathShoppingCartItems);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null
                ? Result<ShoppingCart?>.Success(null)
                : Result<ShoppingCart?>.Success(Mapper.Map<ShoppingCart>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get cart by user");
            return Result<ShoppingCart?>.Failure("Error getting cart by user");
        }
    }

    public async Task<Result<ShoppingCart>> CreateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartEntity>(cart);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<ShoppingCart>.Success(cart) : Result<ShoppingCart>.Failure("Create cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating cart");
            return Result<ShoppingCart>.Failure("Error creating cart");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartEntity>(cart);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating cart");
            return Result<bool>.Failure("Error updating cart");
        }
    }

    public async Task<Result<bool>> DeleteShoppingCartAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShoppingCartEntity(cartId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting cart");
            return Result<bool>.Failure("Error deleting cart");
        }
    }

    public async Task<Result<bool>> ClearShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<bool>.Failure("Invalid userId");
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var cartQuery = qf.Create<ShoppingCartEntity>().Where(ShoppingCartFields.UserId == userId);
            var cart = await adapter.FetchFirstAsync(cartQuery, cancellationToken);
            if (cart == null) return Result<bool>.Success(true);

            var bucket = new RelationPredicateBucket(ShoppingCartItemFields.CartId == cart.CartId);
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(ShoppingCartItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected >= 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing cart");
            return Result<bool>.Failure("Error clearing cart");
        }
    }

    // Shopping Cart Item operations
    public async Task<Result<PagedResult<ShoppingCartItem>>> GetShoppingCartItemsAsync(PagedRequest request, Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            request.AvailableFields = GetSearchableFields();
            var validation = request.Validate();
            if (validation != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return Result<PagedResult<ShoppingCartItem>>.Failure(validation?.ErrorMessage ?? "Invalid paged request");
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var baseQuery = qf.Create<ShoppingCartItemEntity>().Where(ShoppingCartItemFields.CartId == cartId);
            var countQuery = baseQuery.Select(() => Functions.CountRow());
            var total = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);

            // sorting
            if (request.HasSorting)
            {
                var sort = request.SortBy?.ToLower();
                var dir = request.SortDirection;
                var map = new Dictionary<string, EntityField2>
                {
                    ["createdat"] = ShoppingCartItemFields.CreatedAt,
                    ["quantity"] = ShoppingCartItemFields.Quantity,
                    ["unitprice"] = ShoppingCartItemFields.UnitPrice
                };
                if (sort != null && map.TryGetValue(sort, out var field))
                {
                    baseQuery = dir == SortDirection.Descending ? baseQuery.OrderBy(field.Descending()) : baseQuery.OrderBy(field.Ascending());
                }
            }
            else
            {
                baseQuery = baseQuery.OrderBy(ShoppingCartItemFields.CreatedAt.Descending());
            }

            baseQuery = baseQuery.Page(request.Page, request.PageSize);
            var entities = await adapter.FetchQueryAsync(baseQuery, cancellationToken);
            var models = Mapper.Map<List<ShoppingCartItem>>(entities);
            return Result<PagedResult<ShoppingCartItem>>.Success(new PagedResult<ShoppingCartItem>(models, total, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error paging cart items");
            return Result<PagedResult<ShoppingCartItem>>.Failure("Error paging cart items");
        }
    }

    public async Task<Result<ShoppingCartItem?>> GetShoppingCartItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>().Where(ShoppingCartItemFields.CartItemId == itemId);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null ? Result<ShoppingCartItem?>.Success(null) : Result<ShoppingCartItem?>.Success(Mapper.Map<ShoppingCartItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get cart item by id");
            return Result<ShoppingCartItem?>.Failure("Error get cart item by id");
        }
    }

    public async Task<Result<ShoppingCartItem?>> GetShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>()
                .Where(ShoppingCartItemFields.CartId == cartId & ShoppingCartItemFields.ProductId == productId);
            if (variantId.HasValue)
                query = query.Where(ShoppingCartItemFields.ProductVariantId == variantId.Value);
            else
                query = query.Where(ShoppingCartItemFields.ProductVariantId.IsNull());
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null ? Result<ShoppingCartItem?>.Success(null) : Result<ShoppingCartItem?>.Success(Mapper.Map<ShoppingCartItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get cart item by product");
            return Result<ShoppingCartItem?>.Failure("Error get cart item by product");
        }
    }

    public async Task<Result<ShoppingCartItem>> AddShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartItemEntity>(item);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<ShoppingCartItem>.Success(item) : Result<ShoppingCartItem>.Failure("Add item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding cart item");
            return Result<ShoppingCartItem>.Failure("Error adding cart item");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShoppingCartItemEntity>(item);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating cart item");
            return Result<bool>.Failure("Error updating cart item");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartItemQuantityAsync(Guid itemId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShoppingCartItemEntity(itemId) { Quantity = quantity };
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update quantity failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating item quantity");
            return Result<bool>.Failure("Error updating item quantity");
        }
    }

    public async Task<Result<bool>> RemoveShoppingCartItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShoppingCartItemEntity(itemId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Remove item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cart item");
            return Result<bool>.Failure("Error removing cart item");
        }
    }

    public async Task<Result<bool>> RemoveShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(ShoppingCartItemFields.CartId == cartId);
            bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductId == productId);
            if (variantId.HasValue)
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId == variantId.Value);
            else
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId.IsNull());
            var affected = await adapter.DeleteEntitiesDirectlyAsync(typeof(ShoppingCartItemEntity), bucket, cancellationToken);
            return Result<bool>.Success(affected > 0);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error remove item by product");
            return Result<bool>.Failure("Error remove item by product");
        }
    }

    // Wishlist operations
    public async Task<Result<Wishlist?>> GetWishlistByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistEntity>().Where(WishlistFields.UserId == userId & WishlistFields.IsDefault == true);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null ? Result<Wishlist?>.Success(null) : Result<Wishlist?>.Success(Mapper.Map<Wishlist>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get wishlist by user");
            return Result<Wishlist?>.Failure("Error get wishlist by user");
        }
    }

    public async Task<Result<Wishlist>> CreateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistEntity>(wishlist);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<Wishlist>.Success(wishlist) : Result<Wishlist>.Failure("Create wishlist failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error create wishlist");
            return Result<Wishlist>.Failure("Error create wishlist");
        }
    }

    public async Task<Result<bool>> UpdateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistEntity>(wishlist);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update wishlist failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error update wishlist");
            return Result<bool>.Failure("Error update wishlist");
        }
    }

    public async Task<Result<bool>> DeleteWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new WishlistEntity(wishlistId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete wishlist failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error delete wishlist");
            return Result<bool>.Failure("Error delete wishlist");
        }
    }

    // Wishlist Item operations
    public async Task<Result<PagedResult<WishlistItem>>> GetWishlistItemsAsync(PagedRequest request, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            request.AvailableFields = GetSearchableFields();
            var validation = request.Validate();
            if (validation != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                return Result<PagedResult<WishlistItem>>.Failure(validation?.ErrorMessage ?? "Invalid paged request");
            }
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var baseQuery = qf.Create<WishlistItemEntity>().Where(WishlistItemFields.WishlistId == wishlistId);
            var total = await adapter.FetchScalarAsync<int>(baseQuery.Select(() => Functions.CountRow()), cancellationToken);
            if (request.HasSorting)
            {
                var sort = request.SortBy?.ToLower();
                var dir = request.SortDirection;
                var map = new Dictionary<string, EntityField2>
                {
                    ["createdat"] = WishlistItemFields.CreatedAt
                };
                if (sort != null && map.TryGetValue(sort, out var field))
                {
                    baseQuery = dir == SortDirection.Descending ? baseQuery.OrderBy(field.Descending()) : baseQuery.OrderBy(field.Ascending());
                }
            }
            else
            {
                baseQuery = baseQuery.OrderBy(WishlistItemFields.CreatedAt.Descending());
            }
            baseQuery = baseQuery.Page(request.Page, request.PageSize);
            var entities = await adapter.FetchQueryAsync(baseQuery, cancellationToken);
            var models = Mapper.Map<List<WishlistItem>>(entities);
            return Result<PagedResult<WishlistItem>>.Success(new PagedResult<WishlistItem>(models, total, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error paging wishlist items");
            return Result<PagedResult<WishlistItem>>.Failure("Error paging wishlist items");
        }
    }

    public async Task<Result<WishlistItem?>> GetWishlistItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistItemEntity>().Where(WishlistItemFields.WishlistItemId == itemId);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null ? Result<WishlistItem?>.Success(null) : Result<WishlistItem?>.Success(Mapper.Map<WishlistItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get wishlist item by id");
            return Result<WishlistItem?>.Failure("Error get wishlist item by id");
        }
    }

    public async Task<Result<WishlistItem?>> GetWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
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
            return entity is null ? Result<WishlistItem?>.Success(null) : Result<WishlistItem?>.Success(Mapper.Map<WishlistItem>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get wishlist item by product");
            return Result<WishlistItem?>.Failure("Error get wishlist item by product");
        }
    }

    public async Task<Result<WishlistItem>> AddWishlistItemAsync(WishlistItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistItemEntity>(item);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<WishlistItem>.Success(item) : Result<WishlistItem>.Failure("Add wishlist item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error add wishlist item");
            return Result<WishlistItem>.Failure("Error add wishlist item");
        }
    }

    public async Task<Result<bool>> RemoveWishlistItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new WishlistItemEntity(itemId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Remove wishlist item failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error remove wishlist item");
            return Result<bool>.Failure("Error remove wishlist item");
        }
    }

    public async Task<Result<bool>> RemoveWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
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
            logger.LogError(ex, "Error remove wishlist item by product");
            return Result<bool>.Failure("Error remove wishlist item by product");
        }
    }

    public async Task<Result<bool>> IsProductInWishlistAsync(Guid userId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistItemEntity>()
                .Where(WishlistItemFields.ProductId == productId)
                .Where(WishlistItemFields.WishlistId.In(
                    qf.Wishlist.Where(WishlistFields.UserId == userId).Select(WishlistFields.WishlistId)
                ));
            if (variantId.HasValue)
                query = query.Where(WishlistItemFields.ProductVariantId == variantId.Value);
            else
                query = query.Where(WishlistItemFields.ProductVariantId.IsNull());
            var found = await adapter.FetchFirstAsync(query, cancellationToken);
            return Result<bool>.Success(found != null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error check product in wishlist");
            return Result<bool>.Failure("Error check product in wishlist");
        }
    }

    // Abandoned Cart operations
    public async Task<Result<PagedResult<AbandonedCart>>> GetAbandonedCartsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var baseQuery = qf.Create<AbandonedCartEntity>();
            var total = await adapter.FetchScalarAsync<int>(baseQuery.Select(() => Functions.CountRow()), cancellationToken);
            baseQuery = baseQuery.OrderBy(AbandonedCartFields.AbandonedAt.Descending()).Page(request.Page, request.PageSize);
            var entities = await adapter.FetchQueryAsync(baseQuery, cancellationToken);
            var models = Mapper.Map<List<AbandonedCart>>(entities);
            return Result<PagedResult<AbandonedCart>>.Success(new PagedResult<AbandonedCart>(models, total, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error paging abandoned carts");
            return Result<PagedResult<AbandonedCart>>.Failure("Error paging abandoned carts");
        }
    }

    public async Task<Result<AbandonedCart?>> GetAbandonedCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<AbandonedCartEntity>().Where(AbandonedCartFields.UserId == userId);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            return entity is null ? Result<AbandonedCart?>.Success(null) : Result<AbandonedCart?>.Success(Mapper.Map<AbandonedCart>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get abandoned cart by user");
            return Result<AbandonedCart?>.Failure("Error get abandoned cart by user");
        }
    }

    public async Task<Result<AbandonedCart>> CreateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<AbandonedCartEntity>(abandonedCart);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            return saved ? Result<AbandonedCart>.Success(abandonedCart) : Result<AbandonedCart>.Failure("Create abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error create abandoned cart");
            return Result<AbandonedCart>.Failure("Error create abandoned cart");
        }
    }

    public async Task<Result<bool>> UpdateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<AbandonedCartEntity>(abandonedCart);
            entity.IsNew = false;
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            return updated ? Result<bool>.Success(true) : Result<bool>.Failure("Update abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error update abandoned cart");
            return Result<bool>.Failure("Error update abandoned cart");
        }
    }

    public async Task<Result<bool>> DeleteAbandonedCartAsync(Guid abandonedCartId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new AbandonedCartEntity(abandonedCartId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete abandoned cart failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error delete abandoned cart");
            return Result<bool>.Failure("Error delete abandoned cart");
        }
    }

    public async Task<Result<PagedResult<AbandonedCart>>> GetAbandonedCartsByDateRangeAsync(PagedRequest request, DateTime fromDate, DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var baseQuery = qf.Create<AbandonedCartEntity>().Where(AbandonedCartFields.AbandonedAt >= fromDate & AbandonedCartFields.AbandonedAt <= toDate);
            var total = await adapter.FetchScalarAsync<int>(baseQuery.Select(() => Functions.CountRow()), cancellationToken);
            baseQuery = baseQuery.OrderBy(AbandonedCartFields.AbandonedAt.Descending()).Page(request.Page, request.PageSize);
            var entities = await adapter.FetchQueryAsync(baseQuery, cancellationToken);
            var models = Mapper.Map<List<AbandonedCart>>(entities);
            return Result<PagedResult<AbandonedCart>>.Success(new PagedResult<AbandonedCart>(models, total, request.Page, request.PageSize));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error paging abandoned carts by date range");
            return Result<PagedResult<AbandonedCart>>.Failure("Error paging abandoned carts by date range");
        }
    }
    
    public async Task<Result<decimal>> CalculateCartTotalAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();

            var sumExpression = (ShoppingCartItemFields.UnitPrice * ShoppingCartItemFields.Quantity)
                .Sum();

            var filterExpression = ShoppingCartItemFields.CartId == cartId;

            var qf = new QueryFactory();
            var query = qf.Create<ShoppingCartItemEntity>()
                .Where(filterExpression)
                .Select(sumExpression);

            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            return Result<decimal>.Success(total ?? 0m);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculate cart total");
            return Result<decimal>.Failure("Error calculate cart total");
        }
    }

    public async Task<Result<int>> GetCartItemCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            // Tính số lượng items trong cart của user
            var cartQuery = qf.Create<ShoppingCartEntity>().Where(ShoppingCartFields.UserId == userId).Select(ShoppingCartFields.CartId);
            var countQuery = qf.Create<ShoppingCartItemEntity>().Where(ShoppingCartItemFields.CartId.In(cartQuery)).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error get cart item count");
            return Result<int>.Failure("Error get cart item count");
        }
    }

    public Task<Result<bool>> ValidateCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        // Tối giản: luôn true (placeholder), có thể mở rộng kiểm tra tồn kho/giá
        return Task.FromResult(Result<bool>.Success(true));
    }
}