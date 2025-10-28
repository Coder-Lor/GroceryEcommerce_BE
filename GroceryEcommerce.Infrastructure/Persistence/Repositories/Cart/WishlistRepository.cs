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

public class WishlistRepository(
    DataAccessAdapter adapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<WishlistRepository> logger
): BasePagedRepository<WishlistEntity, Wishlist>(adapter, unitOfWorkService, mapper, cacheService, logger), IWishlistRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Name", typeof(string)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("IsDefault", typeof(bool)),
            new SearchableField("IsPublic", typeof(bool)),
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
            new FieldMapping { FieldName = "WishlistId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsDefault", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsPublic", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["wishlistid"] = WishlistFields.WishlistId,
            ["userid"] = WishlistFields.UserId,
            ["name"] = WishlistFields.Name,
            ["isdefault"] = WishlistFields.IsDefault,
            ["ispublic"] = WishlistFields.IsPublic,
            ["createdat"] = WishlistFields.CreatedAt,
            ["updatedat"] = WishlistFields.UpdatedAt
        };
    }

    protected override EntityQuery<WishlistEntity> ApplySearch(EntityQuery<WishlistEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        var term = searchTerm.Trim().ToLower();
        return query.Where(WishlistFields.Name.Contains(term));
    }

    protected override EntityQuery<WishlistEntity> ApplySorting(EntityQuery<WishlistEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var map = GetFieldMap();
        if (!map.TryGetValue(sortBy.ToLower(), out var field)) return query;
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<WishlistEntity> ApplyDefaultSorting(EntityQuery<WishlistEntity> query)
    {
        return query.OrderBy(WishlistFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return WishlistFields.WishlistId;
    }

    protected override object GetEntityId(WishlistEntity entity, EntityField2 primaryKeyField)
    {
        return entity.WishlistId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<Wishlist?>> GetByIdAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(WishlistFields.WishlistId, wishlistId, "Wishlist", TimeSpan.FromMinutes(30), cancellationToken);
    }

    public Task<Result<PagedResult<Wishlist>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("UserId", userId), cancellationToken: cancellationToken);
    }

    public Task<Result<Wishlist?>> GetDefaultWishlistByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return GetSingleAsync(WishlistFields.UserId, userId, "DefaultWishlist_User_" + userId, TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<Wishlist>> CreateAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<WishlistEntity>(wishlist);
            entity.IsNew = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<Wishlist>.Failure("Create wishlist failed");
            return Result<Wishlist>.Success(wishlist);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating wishlist");
            return Result<Wishlist>.Failure("Error creating wishlist");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
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
            logger.LogError(ex, "Error updating wishlist");
            return Result<bool>.Failure("Error updating wishlist");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (wishlistId == Guid.Empty) return Result<bool>.Failure("Invalid wishlistId");
            var adapter = GetAdapter();
            var entity = new WishlistEntity(wishlistId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            return deleted ? Result<bool>.Success(true) : Result<bool>.Failure("Delete wishlist failed");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting wishlist");
            return Result<bool>.Failure("Error deleting wishlist");
        }
    }

    public async Task<Result<bool>> SetDefaultWishlistAsync(Guid userId, Guid wishlistId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || wishlistId == Guid.Empty)
                return Result<bool>.Failure("Invalid IDs");

            var adapter = GetAdapter();
            var qf = new QueryFactory();

            var listQuery = qf.Create<WishlistEntity>().Where(WishlistFields.UserId == userId);
            var wishlists = await FetchEntitiesAsync(listQuery, adapter, cancellationToken);

            foreach (var e in wishlists)
            {
                var setDefault = e.WishlistId == wishlistId;
                if (e.IsDefault != setDefault)
                {
                    e.IsDefault = setDefault;
                    e.IsNew = false;
                    await adapter.SaveEntityAsync(e, cancellationToken);
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting default wishlist");
            return Result<bool>.Failure("Error setting default wishlist");
        }
    }

    public async Task<Result<bool>> RemoveDefaultWishlistAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<bool>.Failure("Invalid userId");
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var listQuery = qf.Create<WishlistEntity>().Where(WishlistFields.UserId == userId & WishlistFields.IsDefault == true);
            var wishlists = await FetchEntitiesAsync(listQuery, adapter, cancellationToken);
            foreach (var e in wishlists)
            {
                e.IsDefault = false;
                e.IsNew = false;
                await adapter.SaveEntityAsync(e, cancellationToken);
            }
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing default wishlist");
            return Result<bool>.Failure("Error removing default wishlist");
        }
    }

    public Task<Result<PagedResult<Wishlist>>> GetPublicWishlistsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => r.WithFilter("IsPublic", true), cancellationToken: cancellationToken);
    }

    public Task<Result<PagedResult<Wishlist>>> GetPublicWishlistsByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        return GetPagedConfiguredAsync(request, r => { r.WithFilter("IsPublic", true); r.WithFilter("UserId", userId); }, cancellationToken: cancellationToken);
    }

    public Task<Result<bool>> ExistsAsync(Guid wishlistId, CancellationToken cancellationToken = default)
    {
        return ExistsByCountAsync(WishlistFields.WishlistId, wishlistId, cancellationToken);
    }

    public async Task<Result<int>> GetWishlistCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<WishlistEntity>().Where(WishlistFields.UserId == userId).Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting wishlists");
            return Result<int>.Failure("Error counting wishlists");
        }
    }
}