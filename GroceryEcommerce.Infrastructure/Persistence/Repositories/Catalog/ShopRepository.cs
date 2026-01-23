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

public class ShopRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ShopRepository> logger
) : BasePagedRepository<ShopEntity, Shop>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IShopRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => ShopFields.Name,
            "slug" => ShopFields.Slug,
            "createdat" => ShopFields.CreatedAt,
            "status" => ShopFields.Status,
            "isaccepted" => ShopFields.IsAccepted,
            _ => null
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new("ShopId", typeof(Guid)),
            new("Name", typeof(string)),
            new("Slug", typeof(string)),
            new("Description", typeof(string)),
            new("LogoUrl", typeof(string)),
            new("Status", typeof(short)),
            new("IsAccepted", typeof(bool)),
            new("OwnerUserId", typeof(Guid)),
            new("CreatedAt", typeof(DateTime)),
            new("UpdatedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField() => "Name";

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new()
            {
                FieldName = "ShopId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "Slug", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "Description", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new()
            {
                FieldName = "LogoUrl", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new()
            {
                FieldName = "Status", FieldType = typeof(short), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "IsAccepted", FieldType = typeof(bool), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "OwnerUserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true,
                IsFilterable = true
            },
            new()
            {
                FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true,
                IsFilterable = true
            }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "shopid", ShopFields.ShopId },
            { "name", ShopFields.Name },
            { "slug", ShopFields.Slug },
            { "description", ShopFields.Description },
            { "logourl", ShopFields.LogoUrl },
            { "status", ShopFields.Status },
            { "isaccepted", ShopFields.IsAccepted },
            { "owneruserid", ShopFields.OwnerUserId },
            { "createdat", ShopFields.CreatedAt },
            { "updatedat", ShopFields.UpdatedAt }
        };
    }

    protected override EntityQuery<ShopEntity> ApplySearch(EntityQuery<ShopEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ShopFields.Name,
            ShopFields.Slug,
            ShopFields.Description);

        return query.Where(predicate);
    }

    protected override EntityQuery<ShopEntity> ApplySorting(EntityQuery<ShopEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? ShopFields.Name;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ShopEntity> ApplyDefaultSorting(EntityQuery<ShopEntity> query)
    {
        return query.OrderBy(ShopFields.Name.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField() => ShopFields.ShopId;

    protected override object GetEntityId(ShopEntity entity, EntityField2 primaryKeyField) => entity.ShopId;

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
        => new PredicateExpression(primaryKeyField.In(ids));

    protected override async Task<IList<ShopEntity>> FetchEntitiesAsync(EntityQuery<ShopEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ShopEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public Task<Result<Shop?>> GetByIdAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        if (shopId == Guid.Empty)
        {
            logger.LogWarning("Shop id is required");
            return Task.FromResult(Result<Shop?>.Failure("Invalid shop ID."));
        }

        return GetSingleAsync(ShopFields.ShopId, shopId, "Shop", TimeSpan.FromHours(1), cancellationToken);
    }

    public Task<Result<Shop?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Shop slug is required");
            return Task.FromResult(Result<Shop?>.Failure("Shop slug is required."));
        }

        return GetSingleAsync(ShopFields.Slug, slug.Trim(), "Shop", TimeSpan.FromHours(1), cancellationToken);
    }

    public Task<Result<PagedResult<Shop>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        // Tạo PrefetchPath để include OwnerUser
        var prefetchPath = new PrefetchPath2(EntityType.ShopEntity);
        prefetchPath.Add(ShopEntity.PrefetchPathUser);
        
        return GetPagedConfiguredAsync(request, _ => { }, prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<bool>> CreateAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<ShopEntity>(shop);
            entity.IsNew = true;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                logger.LogWarning("Failed to create shop: {Name}", shop.Name);
                return Result<bool>.Failure("Failed to create shop.");
            }

            // Cập nhật ShopId vào shop object sau khi save
            shop.ShopId = entity.ShopId;

            var cacheKey = $"Shop_{entity.ShopId}";
            await CacheService.RemoveAsync(cacheKey, cancellationToken);
            logger.LogInformation("Shop created: {Name} with ShopId: {ShopId}", shop.Name, shop.ShopId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating shop: {Name}", shop.Name);
            return Result<bool>.Failure("An error occurred while creating the shop.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Shop shop, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<ShopEntity>(shop);
            entity.IsNew = false;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                logger.LogWarning("Failed to update shop: {ShopId}", shop.ShopId);
                return Result<bool>.Failure("Failed to update shop.");
            }

            var cacheKey = $"Shop_{entity.ShopId}";
            await CacheService.RemoveAsync(cacheKey, cancellationToken);
            logger.LogInformation("Shop updated: {ShopId}", shop.ShopId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating shop: {ShopId}", shop.ShopId);
            return Result<bool>.Failure("An error occurred while updating the shop.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (shopId == Guid.Empty)
            {
                logger.LogWarning("Shop id is required");
                return Result<bool>.Failure("Invalid shop ID.");
            }

            var entity = new ShopEntity(shopId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted)
            {
                logger.LogWarning("Failed to delete shop: {ShopId}", shopId);
                return Result<bool>.Failure("Failed to delete shop.");
            }

            var cacheKey = $"Shop_{shopId}";
            await CacheService.RemoveAsync(cacheKey, cancellationToken);
            logger.LogInformation("Shop deleted: {ShopId}", shopId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting shop: {ShopId}", shopId);
            return Result<bool>.Failure("An error occurred while deleting the shop.");
        }
    }

    public Task<Result<PagedResult<Shop>>> GetByOwnerAsync(Guid ownerUserId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        if (ownerUserId == Guid.Empty)
        {
            logger.LogWarning("Owner user id is required");
            return Task.FromResult(Result<PagedResult<Shop>>.Failure("Invalid owner user ID."));
        }

        // Tạo PrefetchPath để include OwnerUser
        var prefetchPath = new PrefetchPath2(EntityType.ShopEntity);
        prefetchPath.Add(ShopEntity.PrefetchPathUser);

        return GetPagedConfiguredAsync(
            request,
            r => r.WithFilter("OwnerUserId", ownerUserId),
            prefetchPath,
            cancellationToken: cancellationToken);
    }

    public Task<Result<PagedResult<Shop>>> GetActiveShopsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        // Tạo PrefetchPath để include OwnerUser
        var prefetchPath = new PrefetchPath2(EntityType.ShopEntity);
        prefetchPath.Add(ShopEntity.PrefetchPathUser);
        
        return GetPagedConfiguredAsync(request, r => r.WithFilter("Status", (short)1), prefetchPath, cancellationToken: cancellationToken);
    }

    public Task<Result<PagedResult<Shop>>> GetPendingShopsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        // Tạo PrefetchPath để include OwnerUser
        var prefetchPath = new PrefetchPath2(EntityType.ShopEntity);
        prefetchPath.Add(ShopEntity.PrefetchPathUser);
        
        return GetPagedConfiguredAsync(request, r => r.WithFilter("IsAccepted", false), prefetchPath, cancellationToken: cancellationToken);
    }

    public Task<Result<bool>> ExistsAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        if (shopId == Guid.Empty)
        {
            logger.LogWarning("Shop id is required");
            return Task.FromResult(Result<bool>.Failure("Invalid shop ID."));
        }

        return ExistsByCountAsync(ShopFields.ShopId, shopId, cancellationToken);
    }

    public Task<Result<bool>> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Shop slug is required");
            return Task.FromResult(Result<bool>.Failure("Shop slug is required."));
        }

        return ExistsByCountAsync(ShopFields.Slug, slug.Trim(), cancellationToken);
    }

    public async Task<Result<int>> GetProductCountByShopAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (shopId == Guid.Empty)
            {
                logger.LogWarning("Shop id is required");
                return Result<int>.Failure("Invalid shop ID.");
            }

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(ProductFields.ShopId == shopId);
            var count = adapter.GetDbCount(new EntityCollection<ProductEntity>(), bucket);
            
            logger.LogInformation("Product count fetched for shop: {ShopId}, Count: {Count}", shopId, count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product count by shop: {ShopId}", shopId);
            return Result<int>.Failure("An error occurred while retrieving product count.");
        }
    }

    public async Task<Result<int>> GetOrderCountByShopAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (shopId == Guid.Empty)
            {
                logger.LogWarning("Shop id is required");
                return Result<int>.Failure("Invalid shop ID.");
            }

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(OrderFields.ShopId == shopId);
            var count = adapter.GetDbCount(new EntityCollection<OrderEntity>(), bucket);
            
            logger.LogInformation("Order count fetched for shop: {ShopId}, Count: {Count}", shopId, count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order count by shop: {ShopId}", shopId);
            return Result<int>.Failure("An error occurred while retrieving order count.");
        }
    }
}


