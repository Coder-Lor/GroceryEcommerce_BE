using System.Collections.ObjectModel;
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Cart;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Cart;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories;

public class CartRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    IUnitOfWorkService unitOfWorkService,
    ICacheService cacheService,
    ILogger<CartRepository> logger): ICartRepository
{
    private readonly DataAccessAdapter _adapter = adapter;
    private readonly IMapper _mapper = mapper;
    private readonly ICacheService _cacheService = cacheService;
    private readonly ILogger<CartRepository> _logger = logger;
    private readonly IUnitOfWorkService _unitOfWorkService = unitOfWorkService;

    public async Task<Result<ShoppingCart?>> GetShoppingCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User id is required");
                return Result<ShoppingCart?>.Failure("User id is required", "USER_ID_REQUIRED");  
            }
            
            string cacheKey = $"cart:{userId}";
            var cart = await _cacheService.GetAsync<ShoppingCart>(cacheKey, cancellationToken);
            if (cart != null)
            {
                _logger.LogInformation("Shopping cart retrieved from cache for user: {UserId}", userId);
                return Result<ShoppingCart?>.Success(cart);
            }
            
            var bucket = new RelationPredicateBucket(ShoppingCartFields.UserId == userId);
            var entity = await Task.Run(() => _adapter.FetchNewEntity<ShoppingCartEntity>(bucket), cancellationToken);
            
            var cartEntity =  entity == null ? null : _mapper.Map<ShoppingCart>(entity);

            if (cartEntity != null)
            {
                await _cacheService.SetAddAsync(cacheKey, cartEntity, cancellationToken);
                _logger.LogInformation("Shopping cart retrieved from database for user: {UserId}", userId);
            }
            _logger.LogInformation("Shopping cart retrieved for user: {UserId}", userId);
            return Result<ShoppingCart?>.Success(cartEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving shopping cart for user: {UserId}", userId);
            return Result<ShoppingCart?>.Failure("An error occurred while retrieving shopping cart", "CART_GET_ERROR");
        }
    }

    public async Task<Result<ShoppingCart>> CreateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<ShoppingCartEntity>(cart);
            entity.IsNew = true;
            var saved = await _adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                _logger.LogError("Error creating shopping cart for user: {UserId}", cart.UserId);
                return Result<ShoppingCart>.Failure("An error occurred while creating shopping cart", "CART_CREATE_ERROR");
            }
            await _cacheService.RemoveAsync($"cart:{cart.UserId}", cancellationToken);
            _logger.LogInformation("Shopping cart created for user: {UserId}", cart.UserId);
            cart.CartId = entity.CartId;
            return Result<ShoppingCart>.Success(cart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating shopping cart for user: {UserId}", cart.UserId);
            return Result<ShoppingCart>.Failure("An error occurred while creating shopping cart", "CART_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<ShoppingCartEntity>(cart);
            var updated = await _adapter.SaveEntityAsync(entity, cancellationToken);
            if (!updated)
            {
                _logger.LogError("Error updating shopping cart for user: {UserId}", cart.UserId);
                return Result<bool>.Failure("An error occurred while updating shopping cart", "CART_UPDATE_ERROR");
            }
            await _cacheService.RemoveAsync($"cart:{cart.UserId}", cancellationToken);
            _logger.LogInformation("Shopping cart updated for user: {UserId}", cart.UserId);
            return Result<bool>.Success(true);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shopping cart for user: {UserId}", cart.UserId);
            return Result<bool>.Failure("An error occurred while updating shopping cart", "CART_UPDATE_ERROR");
        }
    }

    public async Task<Result<bool>> DeleteShoppingCartAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cartId == Guid.Empty)
            {
                _logger.LogWarning("Cart id is required");
                return Result<bool>.Failure("Cart id is required", "CART_ID_REQUIRED");
            }
            
            var entity = new ShoppingCartEntity(cartId);
            var result = await _adapter.DeleteEntityAsync(entity, cancellationToken);

            if (result)
            {
                await _cacheService.RemoveAsync($"cart:{cartId}", cancellationToken);
                _logger.LogInformation("Shopping cart deleted for user: {UserId}", cartId);
                return Result<bool>.Success(true);
            }
            _logger.LogWarning("Failed to delete shopping cart for user: {UserId}", cartId);
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting shopping cart for user: {UserId}", cartId);
            return Result<bool>.Failure("An error occurred while deleting shopping cart", "CART_DELETE_ERROR");
        }
    }

    public async Task<Result<bool>> ClearShoppingCartAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User id is required");
                return Result<bool>.Failure("User id is required", "USER_ID_REQUIRED");
            }
            
            var entity = new ShoppingCartEntity(userId);
            var result = await _adapter.DeleteEntityAsync(entity, cancellationToken);
            if (result)
            {
                await _cacheService.RemoveAsync($"cart:{userId}", cancellationToken);
                _logger.LogInformation("Shopping cart cleared for user: {UserId}", userId);
                return Result<bool>.Success(true);
            }
            _logger.LogWarning("Failed to clear shopping cart for user: {UserId}", userId);
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing shopping cart for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while clearing shopping cart", "CART_CLEAR_ERROR");
        }
    }

    public async Task<Result<List<ShoppingCartItem>>> GetShoppingCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (cartId == Guid.Empty)
            {
                _logger.LogWarning("Cart id is required");
                return Result<List<ShoppingCartItem>>.Failure("Cart id is required", "CART_ID_REQUIRED");
            }
            var cacheKey = $"cart:{cartId}:items";
            var items = await _cacheService.GetAsync<List<ShoppingCartItem>>(cacheKey, cancellationToken);
            if (items != null)
            {
                _logger.LogInformation("Shopping cart items retrieved from cache for cart: {CartId}", cartId);
                return Result<List<ShoppingCartItem>>.Success(items);
            }
            
            var cartItems = new EntityCollection<ShoppingCartItemEntity>();
            var query = new QueryParameters
            {
                CollectionToFetch = cartItems,
                FilterToUse = new PredicateExpression(ShoppingCartItemFields.CartId == cartId),
                PrefetchPathToUse = new PrefetchPath2(EntityType.ShoppingCartItemEntity)
            };
            await _adapter.FetchEntityCollectionAsync(query, cancellationToken);
            items = _mapper.Map<List<ShoppingCartItem>>(cartItems);

            if (items.Count > 0)
            {
                await _cacheService.SetAddAsync(cacheKey, items, cancellationToken);
                _logger.LogInformation("Shopping cart items retrieved from database for cart: {CartId}", cartId);
            }
            
            _logger.LogInformation("Shopping cart items retrieved for cart: {CartId}", cartId);
            return Result<List<ShoppingCartItem>>.Success(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shopping cart items for cart: {CartId}", cartId);
            return Result<List<ShoppingCartItem>>.Failure("An error occurred while getting shopping cart items", "CART_ITEM_GET_ERROR");
        }
    }

    public async Task<Result<ShoppingCartItem?>> GetShoppingCartItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("Item id is required");
                return Result<ShoppingCartItem?>.Failure("Item id is required", "ITEM_ID_REQUIRED");
            }
            
            var cacheKey = $"cart:{itemId}";
            var item = await _cacheService.GetAsync<ShoppingCartItem>(cacheKey, cancellationToken);
            if (item != null)
            {
                _logger.LogInformation("Shopping cart item retrieved from cache for item: {ItemId}", itemId);
                return Result<ShoppingCartItem?>.Success(item);
            }
            
            var entity = new ShoppingCartItemEntity(itemId);
            var fetched = await Task.Run(() => _adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                _logger.LogWarning("Shopping cart item not found for item: {ItemId}", itemId);
                return Result<ShoppingCartItem?>.Failure("Shopping cart item not found", "CART_ITEM_GET_001");
            }
            
            var itemEntity = _mapper.Map<ShoppingCartItem>(entity);
            if (itemEntity != null)
            {
                await _cacheService.SetAddAsync(cacheKey, itemEntity, cancellationToken);
                _logger.LogInformation("Shopping cart item retrieved from database for item: {ItemId}", itemId);
            }
            
            _logger.LogInformation("Shopping cart item retrieved for item: {ItemId}", itemId);
            return Result<ShoppingCartItem?>.Success(itemEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shopping cart item by id: {ItemId}", itemId);
            return Result<ShoppingCartItem?>.Failure("An error occurred while getting shopping cart item", "CART_ITEM_GET_ERROR");
        }
    }

    public async Task<Result<ShoppingCartItem?>> GetShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (cartId == Guid.Empty)
            {
                _logger.LogWarning("Cart id is required");
                return Result<ShoppingCartItem?>.Failure("Cart id is required", "CART_ID_REQUIRED");
            }

            if (productId == Guid.Empty)
            {
                _logger.LogWarning("Product id is required");
                return Result<ShoppingCartItem?>.Failure("Product id is required", "PRODUCT_ID_REQUIRED");
            }
            var cacheKey = $"cart:{cartId}:item:{productId}:{variantId}";
            var item = await _cacheService.GetAsync<ShoppingCartItem>(cacheKey, cancellationToken);
            if (item != null)
            {
                _logger.LogInformation("Shopping cart item retrieved from cache for cart: {CartId} and product: {ProductId}", cartId, productId);
                return Result<ShoppingCartItem?>.Success(item);
            }
            
            var bucket = new RelationPredicateBucket(
                ShoppingCartItemFields.CartId == cartId &
                         ShoppingCartItemFields.ProductId == productId);
            if (variantId.HasValue)
            {
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId == variantId);
            }
            
            var entity = await Task.Run(() => _adapter.FetchNewEntity<ShoppingCartItemEntity>(bucket), cancellationToken);
            
            if (entity == null)
            {
                _logger.LogWarning("Shopping cart item not found for cart: {CartId} and product: {ProductId}", cartId, productId);
                return Result<ShoppingCartItem?>.Failure("Shopping cart item not found", "CART_ITEM_GET_002");
            }
            
            var itemEntity = _mapper.Map<ShoppingCartItem>(entity);
            if (itemEntity != null)
            {
                await _cacheService.SetAddAsync(cacheKey, itemEntity, cancellationToken);
                _logger.LogInformation("Shopping cart item retrieved from database for cart: {CartId} and product: {ProductId}", cartId, productId);
            }
            return Result<ShoppingCartItem?>.Success(itemEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting shopping cart item by product: {ProductId}", productId);
            return Result<ShoppingCartItem?>.Failure("An error occurred while getting shopping cart item",
                "CART_ITEM_GET_ERROR");
        }
    }

    public async Task<Result<ShoppingCartItem>> AddShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var shoppingCartEntity = _mapper.Map<EntityClasses.ShoppingCartEntity>(item);
            shoppingCartEntity.IsNew = true;
            var saved =  await _adapter.SaveEntityAsync(shoppingCartEntity, cancellationToken);

            if (saved)
            {
                await _cacheService.RemoveAsync($"cart:{item.CartId}", cancellationToken);
                await _cacheService.SetAddAsync($"cart:{item.CartId}:items", item, cancellationToken);
                _logger.LogInformation("Shopping cart item added for cart: {CartId}", item.CartId);
                return Result<ShoppingCartItem>.Success(item);
            }
            
            _logger.LogError("Error adding shopping cart item for cart: {CartId}", item.CartId);
            return Result<ShoppingCartItem>.Failure("An error occurred while adding shopping cart item", "CART_ITEM_ADD_ERROR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding shopping cart item");
            return Result<ShoppingCartItem>.Failure("An error occurred while adding shopping cart item", "CART_ITEM_ADD_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartItemAsync(ShoppingCartItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<ShoppingCartItemEntity>(item);
            entity.IsNew = false;
            
            var updated = await _adapter.SaveEntityAsync(entity, cancellationToken);
            if (!updated)
            {
                _logger.LogError("Error updating shopping cart item for cart: {CartId}", item.CartId);
                return Result<bool>.Failure("An error occurred while updating shopping cart item", "CART_ITEM_UPDATE_ERROR");
            }
            await _cacheService.RemoveAsync($"cart:{item.CartId}", cancellationToken);
            await _cacheService.SetAddAsync($"cart:{item.CartId}:items", item, cancellationToken);
            _logger.LogInformation("Shopping cart item updated for cart: {CartId}", item.CartId);
            return Result<bool>.Success(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shopping cart item");
            return Result<bool>.Failure("An error occurred while updating shopping cart item", "CART_ITEM_UPDATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateShoppingCartItemQuantityAsync(Guid itemId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("Item id is required");
                return Result<bool>.Failure("Item id is required", "ITEM_ID_REQUIRED");
            }
            
            if (quantity <= 0)
            {
                _logger.LogWarning("Quantity must be greater than 0");
                return Result<bool>.Failure("Quantity must be greater than 0", "CART_ITEM_QUANTITY_REQUIRED");
            }
            
            var entity = new ShoppingCartItemEntity(itemId);
            var fetched = await Task.Run(() => _adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                _logger.LogWarning("Shopping cart item not found for item: {ItemId}", itemId);
                return Result<bool>.Failure("Shopping cart item not found", "CART_ITEM_GET_003");
            }
            
            entity.Quantity = quantity;
            var updated = await _adapter.SaveEntityAsync(entity, cancellationToken);
            if (!updated)
            {
                _logger.LogError("Error updating shopping cart item quantity for item: {ItemId}", itemId);
                return Result<bool>.Failure("An error occurred while updating shopping cart item quantity",
                    "CART_ITEM_UPDATE_QUANTITY_ERROR");
            }
            await _cacheService.RemoveAsync($"cart:{entity.CartId}", cancellationToken);
            await _cacheService.SetAddAsync($"cart:{entity.CartId}:items", entity, cancellationToken);
            return Result<bool>.Success(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating shopping cart item quantity");
            return Result<bool>.Failure("An error occurred while updating shopping cart item quantity,",
                "CART_ITEM_UPDATE_QUANTITY_ERROR");
        }
    }

    public async Task<Result<bool>> RemoveShoppingCartItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("Item id is required");
                return Result<bool>.Failure("Item id is required", "ITEM_ID_REQUIRED");
            }
            
            var entity = new ShoppingCartItemEntity(itemId);
            var deleted = await _adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
            {
                _logger.LogError("Error removing shopping cart item for item: {ItemId}", itemId);
                return Result<bool>.Failure("An error occurred while removing shopping cart item", "CART_ITEM_REMOVE_ERROR");
            }
            
            await _cacheService.RemoveAsync($"cart:{entity.CartId}", cancellationToken);
            await _cacheService.RemoveAsync($"cart:{entity.CartId}:items", cancellationToken);
            _logger.LogInformation("Shopping cart item removed for item: {ItemId}", itemId);
            return Result<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing shopping cart item");
            return Result<bool>.Failure("An error occurred while removing shopping cart item", "CART_ITEM_REMOVE_ERROR");
        }
    }

    public async Task<Result<bool>> RemoveShoppingCartItemByProductAsync(Guid cartId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (cartId == Guid.Empty)
            {
                _logger.LogWarning("Cart id is required");
                return Result<bool>.Failure("Cart id is required", "CART_ID_REQUIRED");
            }
            
            if (productId == Guid.Empty)
            {
                _logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Product id is required", "PRODUCT_ID_REQUIRED");
            }
            
            var bucket = new RelationPredicateBucket(
                ShoppingCartItemFields.CartId == cartId &
                         ShoppingCartItemFields.ProductId == productId);

            if (variantId.HasValue)
            {
                bucket.PredicateExpression.AddWithAnd(ShoppingCartItemFields.ProductVariantId == variantId);
            }
            
            var count = await _adapter.DeleteEntitiesDirectlyAsync(typeof(ShoppingCartItemEntity), bucket, cancellationToken);
            if (count > 0)
            {
                await _cacheService.RemoveAsync($"cart:{cartId}:items", cancellationToken);
                _logger.LogInformation("Shopping cart item removed for cart: {CartId} and product: {ProductId}", cartId, productId);
                return Result<bool>.Success(true);
            }
            _logger.LogWarning("Shopping cart item not found for cart: {CartId} and product: {ProductId}", cartId, productId);
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing shopping cart item by product");
            return Result<bool>.Failure("An error occurred while removing shopping cart item", "CART_ITEM_REMOVE_ERROR");
        }
    }

    public async Task<Result<Wishlist?>> GetWishlistByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User id is required");
                return Result<Wishlist?>.Failure("User id is required", "USER_ID_REQUIRED");  
            }
            var cacheKey = $"wishlist:{userId}";
            var wishlist = await _cacheService.GetAsync<Wishlist>(cacheKey, cancellationToken);
            if (wishlist != null)
            {
                _logger.LogInformation("Wishlist retrieved from cache for user: {UserId}", userId);
                return Result<Wishlist?>.Success(wishlist);
            }
            
            var entity = new WishlistEntity(userId);
            var fetched = await Task.Run(() => _adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                _logger.LogWarning("Wishlist not found for user: {UserId}", userId);
                return Result<Wishlist?>.Failure("Wishlist not found", "WISHLIST_GET_001");
            }
            
            var wishlistEntity = _mapper.Map<Wishlist>(entity);
            _logger.LogInformation("Wishlist retrieved from database for user: {UserId}", userId);
            
            await _cacheService.SetAddAsync(cacheKey, wishlistEntity, cancellationToken);
            return Result<Wishlist?>.Success(wishlistEntity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wishlist for user: {UserId}", userId);
            return Result<Wishlist?>.Failure("An error occurred while retrieving wishlist", "WISHLIST_GET_ERROR");
        }
    }

    public async Task<Result<Wishlist>> CreateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var wishlistEntity = _mapper.Map<EntityClasses.WishlistEntity>(wishlist);
            wishlistEntity.IsNew = true;
            var saved =  await _adapter.SaveEntityAsync(wishlistEntity, cancellationToken);
            if (saved)
            {
                await _cacheService.RemoveAsync($"wishlist:{wishlist.UserId}", cancellationToken);
                _logger.LogInformation("Wishlist created for user: {UserId}", wishlist.UserId);
                return Result<Wishlist>.Success(wishlist);
            }
            
            _logger.LogError("Error creating wishlist for user: {UserId}", wishlist.UserId);
            return Result<Wishlist>.Failure("An error occurred while creating wishlist", "WISHLIST_CREATE_ERROR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wishlist");
            return Result<Wishlist>.Failure("An error occurred while creating wishlist", "WISHLIST_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateWishlistAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<WishlistEntity>(wishlist);
            entity.IsNew = false;
            
            var updated = await _adapter.SaveEntityAsync(entity, cancellationToken);
            if (!updated)
            {
                _logger.LogError("Error updating wishlist for user: {UserId}", wishlist.UserId);
                return Result<bool>.Failure("An error occurred while updating wishlist", "WISHLIST_UPDATE_ERROR");
            }
            await _cacheService.RemoveAsync($"wishlist:{wishlist.UserId}", cancellationToken);
            _logger.LogInformation("Wishlist updated for user: {UserId}", wishlist.UserId);
            return Result<bool>.Success(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating wishlist");
            return Result<bool>.Failure("An error occurred while updating wishlist", "WISHLIST_UPDATE_ERROR");
        }
    }
    
    

    public async Task<Result<bool>> DeleteWishlistAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty)
            {
                _logger.LogWarning("Wishlist id is required");
                return Result<bool>.Failure("Wishlist id is required", "WISHLIST_ID_REQUIRED");
            }
            
            var entity = new WishlistEntity(wishlistId);
            var deleted = await _adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
            {
                _logger.LogError("Error deleting wishlist for user: {UserId}", wishlistId);
                return Result<bool>.Failure("An error occurred while deleting wishlist", "WISHLIST_DELETE_ERROR");
            }
            await _cacheService.RemoveAsync($"wishlist:{wishlistId}", cancellationToken);
            _logger.LogInformation("Wishlist deleted for user: {UserId}", wishlistId);
            return Result<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting wishlist");
            return Result<bool>.Failure("An error occurred while deleting wishlist", "WISHLIST_DELETE_ERROR");
        }
    }

    public async Task<Result<List<WishlistItem>>> GetWishlistItemsAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty)
            {
                _logger.LogWarning("Wishlist id is required");
                return Result<List<WishlistItem>>.Failure("Wishlist id is required", "WISHLIST_ID_REQUIRED");
            }
            var cacheKey = $"wishlist:{wishlistId}:items";
            var items = await _cacheService.GetAsync<List<WishlistItem>>(cacheKey, cancellationToken);
            if (items != null)
            {
                _logger.LogInformation("Wishlist items retrieved from cache for user: {UserId}", wishlistId);
                return Result<List<WishlistItem>>.Success(items);
            }
            
            var wishlistItemEntities = new EntityCollection<WishlistItemEntity>();
            var filter = new PredicateExpression(WishlistItemFields.WishlistId == wishlistId);

            var query = new QueryParameters()
            {
                CollectionToFetch = wishlistItemEntities,
                FilterToUse = filter,
            };

            await _adapter.FetchEntityCollectionAsync(query, cancellationToken);
            var wishlistItems = _mapper.Map<List<WishlistItem>>(wishlistItemEntities);
            if (wishlistItems != null && wishlistItems.Any())
            {
                await _cacheService.SetAddAsync(cacheKey, wishlistItems, cancellationToken);
                _logger.LogInformation("Wishlist items retrieved from database for user: {UserId}", wishlistId);
                return Result<List<WishlistItem>>.Success(wishlistItems);
            }
            _logger.LogWarning("Wishlist items not found for user: {UserId}", wishlistId);
            return Result<List<WishlistItem>>.Success(new List<WishlistItem>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving wishlist items for user: {UserId}", wishlistId);
            return Result<List<WishlistItem>>.Failure("An error occurred while retrieving wishlist items", "WISHLIST_ITEMS_GET_ERROR");;
        }
    }

    public async Task<Result<WishlistItem?>> GetWishlistItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("Item id is required");
                return Result<WishlistItem?>.Failure("Item id is required", "ITEM_ID_REQUIRED");
            }
            
            var cacheKey = $"wishlistitem:{itemId}";
            var item = await _cacheService.GetAsync<WishlistItem>(cacheKey, cancellationToken);
            if (item != null)
            {
                _logger.LogInformation("Wishlist item retrieved from cache for item: {ItemId}", itemId);
                return Result<WishlistItem?>.Success(item);
            }
            
            var entity = new WishlistItemEntity(itemId);
            var fetched = await Task.Run(() => _adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                _logger.LogWarning("Wishlist item not found for item: {ItemId}", itemId);
                return Result<WishlistItem?>.Failure("Wishlist item not found", "WISHLIST_ITEM_GET_001");
            }
            
            var wishlistItems = _mapper.Map<WishlistItem>(entity);
            _logger.LogInformation("Wishlist item retrieved from database for item: {ItemId}", itemId);
            return Result<WishlistItem?>.Success(wishlistItems);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist item by id");
            return Result<WishlistItem?>.Failure("An error occurred while getting wishlist item", "WISHLIST_ITEM_GET_ERROR");
        }
    }

    public async Task<Result<WishlistItem?>> GetWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty)
            {
                _logger.LogWarning("Wishlist id is required");
                return Result<WishlistItem?>.Failure("Wishlist id is required", "WISHLIST_ID_REQUIRED");
            }

            if (productId == Guid.Empty)
            {
                _logger.LogWarning("Product id is required");
                return Result<WishlistItem?>.Failure("Product id is required", "PRODUCT_ID_REQUIRED");
            }

            var cacheKey = $"wishlist:{wishlistId}:item:{productId}:{variantId}";
            var item = await _cacheService.GetAsync<WishlistItem>(cacheKey, cancellationToken);

            if (item != null)
            {
                _logger.LogInformation("Wishlist item retrieved from cache for cart: {CartId} and product: {ProductId}", wishlistId, productId);
                return Result<WishlistItem?>.Success(item);
            }
            var bucket = new RelationPredicateBucket(
                WishlistItemFields.WishlistId == wishlistId &
                         WishlistItemFields.ProductId == productId);
            if (variantId.HasValue) 
                bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductVariantId == variantId);

            var entity = await Task.Run(() => _adapter.FetchNewEntity<WishlistItemEntity>(bucket), cancellationToken);
            if (entity == null)
            {
                _logger.LogWarning("Wishlist item not found for wishlist: {WishlistId} and product: {ProductId}", wishlistId, productId);
                return Result<WishlistItem?>.Failure("Wishlist item not found", "WISHLIST_ITEM_GET_002");
            }
            var wishlistItem = _mapper.Map<WishlistItem>(entity);
            _logger.LogInformation("Wishlist item retrieved from database for wishlist: {WishlistId} and product: {ProductId}", wishlistId, productId);
            await _cacheService.SetAddAsync(cacheKey, wishlistItem, cancellationToken);
            return Result<WishlistItem?>.Success(wishlistItem);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting wishlist item by product");
            return Result<WishlistItem?>.Failure("An error occurred while getting wishlist item", "WISHLIST_ITEM_GET_ERROR");
        }
    }

    public async Task<Result<WishlistItem>> AddWishlistItemAsync(WishlistItem item, CancellationToken cancellationToken = default)
    {
        try
        {
            var wishlistItemEntity = _mapper.Map<EntityClasses.WishlistItemEntity>(item);
            wishlistItemEntity.IsNew = true;
            var saved =  await _adapter.SaveEntityAsync(wishlistItemEntity, cancellationToken);
            if (saved)
            {
                await _cacheService.RemoveAsync($"wishlist:{item.WishlistId}:items", cancellationToken);
                _logger.LogInformation("Wishlist item created for wishlist: {WishlistId} and product: {ProductId}", item.WishlistId, item.ProductId);
                return Result<WishlistItem>.Success(item);
            }
            
            _logger.LogError("Error creating wishlist item for wishlist: {WishlistId} and product: {ProductId}", item.WishlistId, item.ProductId);
            return Result<WishlistItem>.Failure("An error occurred while creating wishlist item", "WISHLIST_ITEM_CREATE_ERROR");
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating wishlist item");
            return Result<WishlistItem>.Failure("An error occurred while creating wishlist item", "WISHLIST_ITEM_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> RemoveWishlistItemAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (itemId == Guid.Empty)
            {
                _logger.LogWarning("Item id is required");
                return Result<bool>.Failure("Item id is required", "ITEM_ID_REQUIRED");
            }
            var entity = new WishlistItemEntity(itemId);
            var deleted = await _adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
            {
                _logger.LogError("Error removing wishlist item for item: {ItemId}", itemId);
                return Result<bool>.Failure("An error occurred while removing wishlist item", "WISHLIST_ITEM_REMOVE_ERROR");
            }
            await _cacheService.RemoveAsync($"wishlist:{entity.WishlistId}:items", cancellationToken);
            _logger.LogInformation("Wishlist item removed for item: {ItemId}", itemId);
            return Result<bool>.Success(deleted);
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing wishlist item");
            return Result<bool>.Failure("An error occurred while removing wishlist item", "WISHLIST_ITEM_REMOVE_ERROR");
        }
    }

    public async Task<Result<bool>> RemoveWishlistItemByProductAsync(Guid wishlistId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty)
            {
                _logger.LogWarning("Wishlist id is required");
                return Result<bool>.Failure("Wishlist id is required", "WISHLIST_ID_REQUIRED");
            }
            if (productId == Guid.Empty)
            {
                _logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Product id is required", "PRODUCT_ID_REQUIRED");
            }
            var bucket = new RelationPredicateBucket(
                WishlistItemFields.WishlistId == wishlistId &
                         WishlistItemFields.ProductId == productId);
            if (variantId.HasValue)
            {
                bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductVariantId == variantId);
            }
            var count = await _adapter.DeleteEntitiesDirectlyAsync(typeof(WishlistItemEntity), bucket, cancellationToken);
            if (count > 0)
            {
                await _cacheService.RemoveAsync($"wishlist:{wishlistId}:items", cancellationToken);
                _logger.LogInformation("Wishlist item removed for wishlist: {WishlistId} and product: {ProductId}", wishlistId, productId);
                return Result<bool>.Success(true);
            }
            _logger.LogWarning("Wishlist item not found for wishlist: {WishlistId} and product: {ProductId}", wishlistId, productId);
            return Result<bool>.Success(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing wishlist item by product");
            return Result<bool>.Failure("An error occurred while removing wishlist item", "WISHLIST_ITEM_REMOVE_ERROR");       
        }
    }

    public async Task<Result<bool>> IsProductInWishlistAsync(Guid userId, Guid productId, Guid? variantId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User id is required");
                return Result<bool>.Failure("User id is required", "USER_ID_REQUIRED");
            }
            
            if (productId == Guid.Empty)
            {
                _logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Product id is required", "PRODUCT_ID_REQUIRED");
            }
            
            var cacheKey = $"wishlist:{userId}:item:{productId}:{variantId}";
            var item = await _cacheService.GetAsync<WishlistItem>(cacheKey, cancellationToken);
            if (item != null)
            {
                _logger.LogInformation("Wishlist item retrieved from cache for user: {UserId} and product: {ProductId}", userId, productId);
                return Result<bool>.Success(true);
            }
            
            var bucket = new RelationPredicateBucket(WishlistItemFields.WishlistId == userId & WishlistItemFields.ProductId == productId);
            
            if (variantId.HasValue)
            {
                bucket.PredicateExpression.AddWithAnd(WishlistItemFields.ProductVariantId == variantId);
            }

            var entity = await Task.Run(() => _adapter.FetchNewEntity<WishlistItemEntity>(bucket), cancellationToken);
            var isProductInWishlist = entity != null;
            if (isProductInWishlist)
            {
                await _cacheService.SetAddAsync(cacheKey, _mapper.Map<WishlistItem>(entity), cancellationToken);
                _logger.LogInformation("Wishlist item retrieved from database for user: {UserId} and product: {ProductId}", userId, productId);
            }
            return Result<bool>.Success(isProductInWishlist);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing wishlist item by product");
            return Result<bool>.Failure("An error occurred while removing wishlist item", "WISHLIST_ITEM_REMOVE_ERROR");      
        }
    }

    public async Task<Result<List<AbandonedCart>>> GetAbandonedCartsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"abandonedcarts";
            var cachedCarts = await _cacheService.GetAsync<List<AbandonedCart>>(cacheKey, cancellationToken);
            if (cachedCarts != null)
            {
                _logger.LogInformation("Abandoned carts retrieved from cache");
                return Result<List<AbandonedCart>>.Success(cachedCarts);
            }
            
            var abandonedCartEntities = new EntityCollection<AbandonedCartEntity>();

            var query = new QueryParameters()
            {
                CollectionToFetch = abandonedCartEntities,
            };
            await _adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            var abandonedCarts = _mapper.Map<List<AbandonedCart>>(abandonedCartEntities);
            if (abandonedCarts != null && abandonedCarts.Any())
            {
                await _cacheService.SetAddAsync(cacheKey, abandonedCarts, cancellationToken);
                _logger.LogInformation("Abandoned carts retrieved from database");
                return Result<List<AbandonedCart>>.Success(abandonedCarts);
            }
            _logger.LogWarning("Abandoned carts not found");
            return Result<List<AbandonedCart>>.Success(new List<AbandonedCart>());

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving abandoned carts");
            return Result<List<AbandonedCart>>.Failure("An error occurred while retrieving abandoned carts", "WISHLIST_ITEMS_GET_ERROR");
        }
    }

    public async Task<Result<AbandonedCart?>> GetAbandonedCartByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("User id is required");
                return Result<AbandonedCart?>.Failure("User id is required", "USER_ID_REQUIRED");
            }
            var cacheKey = $"abandonedcart:{userId}";
            var cart = await _cacheService.GetAsync<AbandonedCart>(cacheKey, cancellationToken);
            if (cart != null)
            {
                _logger.LogInformation("Abandoned cart retrieved from cache for user: {UserId}", userId);
                return Result<AbandonedCart?>.Success(cart);
            }
            
            var entity = new AbandonedCartEntity(userId);
            var fetched = await Task.Run(() => _adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                _logger.LogWarning("Abandoned cart not found for user: {UserId}", userId);
                return Result<AbandonedCart?>.Failure("Abandoned cart not found", "ABANDONED_CART_GET_001");
            }
            
            var abandonedCart = _mapper.Map<AbandonedCart>(entity);
            _logger.LogInformation("Abandoned cart retrieved from database for user: {UserId}", userId);
            await _cacheService.SetAddAsync(cacheKey, abandonedCart, cancellationToken);
            return Result<AbandonedCart?>.Success(abandonedCart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving abandoned cart by user id");
            return Result<AbandonedCart?>.Failure("An error occurred while retrieving abandoned cart", "WISHLIST_ITEMS_GET_ERROR");
        }
    }

    public async Task<Result<AbandonedCart>> CreateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        try
        {
            var abandonedCartEntity = _mapper.Map<AbandonedCartEntity>(abandonedCart);
            abandonedCartEntity.IsNew = true;
            var saved = await _adapter.SaveEntityAsync(abandonedCartEntity, cancellationToken);
            if (saved)
            {
                await _cacheService.RemoveAsync($"abandonedcart:{abandonedCart.UserId}", cancellationToken);
                _logger.LogInformation("Abandoned cart created for user: {UserId}", abandonedCart.UserId);
                return Result<AbandonedCart>.Success(abandonedCart);
            }
            _logger.LogError("Error creating abandoned cart for user: {UserId}", abandonedCart.UserId);
            return Result<AbandonedCart>.Failure("An error occurred while creating abandoned cart", "ABANDONED_CART_CREATE_ERROR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating abandoned cart");
            return Result<AbandonedCart>.Failure("An error occurred while creating abandoned cart", "ABANDONED_CART_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateAbandonedCartAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default)
    {
        try
        {
            var abandonedCartEntity = _mapper.Map<AbandonedCartEntity>(abandonedCart);
            abandonedCartEntity.IsNew = false;
            var saved = await _adapter.SaveEntityAsync(abandonedCartEntity, cancellationToken);
            if (saved)
            {
                await _cacheService.RemoveAsync($"abandonedcart:{abandonedCart.UserId}", cancellationToken);
                _logger.LogInformation("Abandoned cart updated for user: {UserId}", abandonedCart.UserId);
                return Result<bool>.Success(saved);
            }
            _logger.LogError("Error updating abandoned cart for user: {UserId}", abandonedCart.UserId);
            return Result<bool>.Failure("An error occurred while updating abandoned cart", "ABANDONED_CART_UPDATE_ERROR");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating abandoned cart");
            return Result<bool>.Failure("An error occurred while updating abandoned cart", "ABANDONED_CART_UPDATE_ERROR");
        }
    }

    public async Task<Result<bool>> DeleteAbandonedCartAsync(Guid abandonedCartId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (abandonedCartId == Guid.Empty)
            {
                _logger.LogWarning("abandoned cart id is required");
                return Result<bool>.Failure("abandoned cart id is required", "ABANDONED_CART_ID_REQUIRED");
            }
            var entity = new AbandonedCartEntity(abandonedCartId);
            var deleted = await _adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
            {
                _logger.LogError("Error deleting abandoned cart for abandoned cart id: {AbandonedCartId}", abandonedCartId);
                return Result<bool>.Failure("An error occurred while deleting abandoned cart", "ABANDONED_CART_DELETE_ERROR");
            }
            _logger.LogInformation("Abandoned cart deleted for abandoned cart id: {AbandonedCartId}", abandonedCartId);
            return Result<bool>.Success(deleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting abandoned cart");
            return Result<bool>.Failure("An error occurred while deleting abandoned cart", "ABANDONED_CART_DELETE_ERROR");
        }
    }

    
    public async Task<Result<List<AbandonedCart>>> GetAbandonedCartsByDateRangeAsync(DateTime fromDate, DateTime toDate,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucket = new RelationPredicateBucket();
            
            // Lọc theo khoảng thời gian
            bucket.PredicateExpression.Add(AbandonedCartFields.AbandonedAt >= fromDate);
            bucket.PredicateExpression.Add(AbandonedCartFields.AbandonedAt <= toDate);
            
            var abandonedCartEntities = await _adapter.FetchQueryAsync<AbandonedCartEntity>(
                new QueryFactory().AbandonedCart
                    .Where(bucket.PredicateExpression),
                cancellationToken);
            
            var abandonedCarts = _mapper.Map<List<AbandonedCart>>(abandonedCartEntities);
            
            return Result<List<AbandonedCart>>.Success(abandonedCarts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting abandoned carts by date range");
            return Result<List<AbandonedCart>>.Failure("An error occurred while getting abandoned carts by date range",
                "ABANDONED_CARTS_DATE_RANGE_ERROR");
        }
    }

    public async Task<Result<decimal>> CalculateCartTotalAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {

            var cartItems = new EntityCollection<ShoppingCartItemEntity>();
            var query = new QueryParameters()
            {
                CollectionToFetch = cartItems,
                FilterToUse = ShoppingCartItemFields.CartId == cartId
            };
            await _adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            var total = cartItems.Sum(item => item.Quantity * item.UnitPrice);

            return Result<decimal>.Success(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating cart total for cart {CartId}", cartId);
            return Result<decimal>.Failure("An error occurred while calculating cart total",
                "CALCULATE_CART_TOTAL_ERROR");
        }
    }

    public async Task<Result<int>> GetCartItemCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
        
            // Tìm giỏ hàng của user
            var cartBucket = new RelationPredicateBucket(ShoppingCartFields.UserId == userId);
        
            var cart = await Task.Run(() => _adapter.FetchNewEntity<ShoppingCartEntity>(cartBucket), cancellationToken);
        
            if (cart == null)
            {
                return Result<int>.Success(0);
            }
        
            // Đếm số lượng items trong giỏ hàng
            var itemBucket = new RelationPredicateBucket(ShoppingCartItemFields.CartId == cart.CartId);
        
            var count = _adapter.GetDbCount(new EntityCollection<ShoppingCartItemEntity>(), itemBucket);
        
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart item count for user {UserId}", userId);
            return Result<int>.Failure("An error occurred while getting cart item count",
                "GET_CART_ITEM_COUNT_ERROR");
        }
    }

    public async Task<Result<bool>> ValidateCartItemsAsync(Guid cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Lấy tất cả items trong giỏ hàng kèm thông tin sản phẩm
            var cartItems = new EntityCollection<ShoppingCartItemEntity>();
            
            var prefetchPath = new PrefetchPath2(EntityType.ShoppingCartItemEntity)
            {
                ShoppingCartItemEntity.PrefetchPathProduct
            };
            
            var query = new QueryParameters()
            {
                CollectionToFetch = cartItems,
                FilterToUse = new PredicateExpression(ShoppingCartItemFields.CartId == cartId),
                PrefetchPathToUse = prefetchPath
            };
                
            await _adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            if (cartItems.Count == 0)
            {
                return Result<bool>.Success(true);
            }
            
            // Kiểm tra từng item
            foreach (var item in cartItems)
            {
                var product = item.Product;
                
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found in cart {CartId}", item.ProductId, cartId);
                    return Result<bool>.Failure(
                        $"Product with ID {item.ProductId} no longer exists",
                        "PRODUCT_NOT_FOUND");
                }
                
                // Kiểm tra sản phẩm còn active không
                if (product.Status != 1)
                {
                    _logger.LogWarning("Product {ProductId} is inactive in cart {CartId}", item.ProductId, cartId);
                    return Result<bool>.Failure(
                        $"Product {product.Name} is no longer available",
                        "PRODUCT_INACTIVE");
                }
                
                // Kiểm tra tồn kho
                if (product.StockQuantity < item.Quantity)
                {
                    _logger.LogWarning("Insufficient stock for product {ProductId} in cart {CartId}. Required: {Required}, Available: {Available}", 
                        item.ProductId, cartId, item.Quantity, product.StockQuantity);
                    return Result<bool>.Failure(
                        $"Insufficient stock for product {product.Name}. Available: {product.StockQuantity}",
                        "INSUFFICIENT_STOCK");
                }
            }
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating cart items for cart {CartId}", cartId);
            return Result<bool>.Failure("An error occurred while validating cart items",
                "VALIDATE_CART_ITEMS_ERROR");
        }
    }
}