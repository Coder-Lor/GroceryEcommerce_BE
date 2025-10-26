using GroceryEcommerce.HelperClasses;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class UserAddressRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<UserAddressRepository> logger 
) : BasePagedRepository<UserAddressEntity, UserAddress>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IUserAddressRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("AddressLine1", typeof(string)),
            new SearchableField("City", typeof(string)),
            new SearchableField("State", typeof(string)),
            new SearchableField("Country", typeof(string)),
            new SearchableField("ZipCode", typeof(string)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("IsDefault", typeof(bool)),
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
            new FieldMapping { FieldName = "AddressLine1", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "City", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "State", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Country", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ZipCode", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AddressType", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsDefault", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "AddressLine1", UserAddressFields.AddressLine1 },
            { "AddressLine2", UserAddressFields.AddressLine2 },
            { "AddressType", UserAddressFields.AddressType },
            { "City", UserAddressFields.City },
            { "State", UserAddressFields.State },
            { "Country", UserAddressFields.Country },
            { "ZipCode", UserAddressFields.ZipCode },
            { "IsDefault", UserAddressFields.IsDefault },
            { "CreatedAt", UserAddressFields.CreatedAt },
            { "UpdatedAt", UserAddressFields.UpdatedAt },
            { "UserId", UserAddressFields.UserId },
            { "AddressId", UserAddressFields.AddressId }
        };
    }

    protected override EntityQuery<UserAddressEntity> ApplySearch(EntityQuery<UserAddressEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        return query.Where(
            UserAddressFields.AddressLine1.Contains(searchTerm) |
            UserAddressFields.AddressLine2.Contains(searchTerm) |
            UserAddressFields.City.Contains(searchTerm) |
            UserAddressFields.State.Contains(searchTerm) |
            UserAddressFields.Country.Contains(searchTerm) |
            UserAddressFields.ZipCode.Contains(searchTerm)
        );
    }

    protected override EntityQuery<UserAddressEntity> ApplySorting(EntityQuery<UserAddressEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var field = sortBy.ToLower() switch
        {
            "addressline1" => UserAddressFields.AddressLine1,
            "city" => UserAddressFields.City,
            "state" => UserAddressFields.State,
            "country" => UserAddressFields.Country,
            "zipcode" => UserAddressFields.ZipCode,
            "addresstype" => UserAddressFields.AddressType,
            "isdefault" => UserAddressFields.IsDefault,
            _ => UserAddressFields.CreatedAt
        };

        return sortDirection == SortDirection.Descending
            ? query.OrderBy(field.Descending())
            : query.OrderBy(field.Ascending());
    }

    protected override EntityQuery<UserAddressEntity> ApplyDefaultSorting(EntityQuery<UserAddressEntity> query)
    {
        return query.OrderBy(UserAddressFields.CreatedAt.Descending());
    }

    protected override async Task<IList<UserAddressEntity>> FetchEntitiesAsync(EntityQuery<UserAddressEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<UserAddressEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return UserAddressFields.AddressId;
    }

    protected override object GetEntityId(UserAddressEntity entity, EntityField2 primaryKeyField)
    {
        return entity.AddressId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public async Task<Result<UserAddress?>> GetByIdAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        if (addressId == Guid.Empty)
        {
            Logger.LogWarning("Address id is required");
            return Result<UserAddress?>.Failure("Invalid address ID.");
        }

        return await GetSingleAsync(UserAddressFields.AddressId, addressId, $"UserAddress_{addressId}", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<UserAddress>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("UserId", userId), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<UserAddress?>> GetDefaultAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<UserAddress?>.Failure("Invalid user id.");

            var cacheKey = $"UserDefaultAddress_{userId}";
            var cached = await CacheService.GetAsync<UserAddress>(cacheKey, cancellationToken);
            if (cached != null) return Result<UserAddress?>.Success(cached);

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserAddressFields.UserId == userId);
            filter.Add(UserAddressFields.IsDefault == true);
            bucket.PredicateExpression.Add(filter);
            var entity = await Task.Run(() => adapter.FetchNewEntity<UserAddressEntity>(bucket), cancellationToken);
            if (entity == null) return Result<UserAddress?>.Success(null);

            var domain = Mapper.Map<UserAddress>(entity);
            await CacheService.SetAsync(cacheKey, domain, TimeSpan.FromHours(1), cancellationToken);
            return Result<UserAddress?>.Success(domain);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching default address for user: {UserId}", userId);
            return Result<UserAddress?>.Failure("An error occurred while retrieving default address.");
        }
    }

    public async Task<Result<UserAddress>> CreateAsync(UserAddress address, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<UserAddressEntity>(address);
            entity.AddressId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsNew = true;

            var adapter = GetAdapter();

            // If new address is default, unset other defaults for the user
            if (entity.IsDefault)
            {
                var bucket = new RelationPredicateBucket(UserAddressFields.UserId == entity.UserId);
                var col = new EntityCollection<UserAddressEntity>();
                var queryParams = new QueryParameters { CollectionToFetch = col, FilterToUse = bucket.PredicateExpression };
                await adapter.FetchEntityCollectionAsync(queryParams, cancellationToken);
                foreach (var a in col)
                {
                    a.IsDefault = false;
                }
                if (col.Count > 0) await adapter.SaveEntityCollectionAsync(col, cancellationToken);
            }

            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<UserAddress>.Failure("Failed to create address.");

            // Invalidate caches
            await CacheService.RemoveAsync($"UserAddresses_ByUser_{entity.UserId}", cancellationToken);
            await CacheService.RemoveAsync($"UserDefaultAddress_{entity.UserId}", cancellationToken);

            Logger.LogInformation("User address created: {AddressId}", entity.AddressId);
            return Result<UserAddress>.Success(Mapper.Map<UserAddress>(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating user address for user: {UserId}", address.UserId);
            return Result<UserAddress>.Failure("An error occurred while creating address.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UserAddress address, CancellationToken cancellationToken = default)
    {
        try
        {
            if (address.AddressId == Guid.Empty) return Result<bool>.Failure("Invalid address id.");

            var entity = new UserAddressEntity(address.AddressId);
            var fetched = await Task.Run(() => GetAdapter().FetchEntity(entity), cancellationToken);
            if (!fetched) return Result<bool>.Failure("Address not found.");

            Mapper.Map(address, entity);
            entity.UpdatedAt = DateTime.UtcNow;

            var adapter = GetAdapter();

            // If setting default, unset others
            if (entity.IsDefault)
            {
                var bucket = new RelationPredicateBucket(UserAddressFields.UserId == entity.UserId);
                var col = new EntityCollection<UserAddressEntity>();
                var queryParams = new QueryParameters { CollectionToFetch = col, FilterToUse = bucket.PredicateExpression };
                await adapter.FetchEntityCollectionAsync(queryParams, cancellationToken);
                foreach (var a in col)
                {
                    if (a.AddressId != entity.AddressId) a.IsDefault = false;
                }
                if (col.Count > 0) await adapter.SaveEntityCollectionAsync(col, cancellationToken);
            }

            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<bool>.Failure("Failed to update address.");

            await CacheService.RemoveAsync($"UserAddresses_ByUser_{entity.UserId}", cancellationToken);
            await CacheService.RemoveAsync($"UserAddress_{entity.AddressId}", cancellationToken);
            await CacheService.RemoveAsync($"UserDefaultAddress_{entity.UserId}", cancellationToken);

            Logger.LogInformation("User address updated: {AddressId}", entity.AddressId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating user address: {AddressId}", address.AddressId);
            return Result<bool>.Failure("An error occurred while updating address.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (addressId == Guid.Empty) return Result<bool>.Failure("Invalid address id.");

            var entity = new UserAddressEntity(addressId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted) return Result<bool>.Failure("Address not found or failed to delete.");

            await CacheService.RemoveAsync($"UserAddresses_ByUser_{entity.UserId}", cancellationToken);
            await CacheService.RemoveAsync($"UserAddress_{addressId}", cancellationToken);
            await CacheService.RemoveAsync($"UserDefaultAddress_{entity.UserId}", cancellationToken);

            Logger.LogInformation("User address deleted: {AddressId}", addressId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting user address: {AddressId}", addressId);
            return Result<bool>.Failure("An error occurred while deleting address.");
        }
    }

    public async Task<Result<bool>> SetDefaultAddressAsync(Guid userId, Guid addressId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || addressId == Guid.Empty) return Result<bool>.Failure("Invalid user or address id.");

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserAddressFields.UserId == userId);
            bucket.PredicateExpression.Add(filter);

            var col = new EntityCollection<UserAddressEntity>();
            var queryParams = new QueryParameters { CollectionToFetch = col, FilterToUse = bucket.PredicateExpression };
            await adapter.FetchEntityCollectionAsync(queryParams, cancellationToken);

            var target = col.FirstOrDefault(a => a.AddressId == addressId);
            if (target == null) return Result<bool>.Failure("Address not found for user.");

            foreach (var a in col)
            {
                a.IsDefault = a.AddressId == addressId;
            }

            var saved = await adapter.SaveEntityCollectionAsync(col, cancellationToken);
            if (saved == 0) return Result<bool>.Failure("Failed to set default address.");

            await CacheService.RemoveAsync($"UserAddresses_ByUser_{userId}", cancellationToken);
            await CacheService.RemoveAsync($"UserDefaultAddress_{userId}", cancellationToken);

            Logger.LogInformation("Default address set for user {UserId}: {AddressId}", userId, addressId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting default address for user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while setting default address.");
        }
    }

    public async Task<Result<bool>> RemoveDefaultAddressAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<bool>.Failure("Invalid user id.");

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserAddressFields.UserId == userId);
            filter.Add(UserAddressFields.IsDefault == true);
            bucket.PredicateExpression.Add(filter);
            var col = new EntityCollection<UserAddressEntity>();
            var queryParams = new QueryParameters { CollectionToFetch = col, FilterToUse = bucket.PredicateExpression };
            await adapter.FetchEntityCollectionAsync(queryParams, cancellationToken);

            if (col.Count == 0) return Result<bool>.Success(true);

            foreach (var a in col)
            {
                a.IsDefault = false;
            }

            var saved = await adapter.SaveEntityCollectionAsync(col, cancellationToken);
            if (saved == 0) return Result<bool>.Failure("Failed to remove default address.");

            await CacheService.RemoveAsync($"UserAddresses_ByUser_{userId}", cancellationToken);
            await CacheService.RemoveAsync($"UserDefaultAddress_{userId}", cancellationToken);

            Logger.LogInformation("Default address removed for user {UserId}", userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing default address for user {UserId}", userId);
            return Result<bool>.Failure("An error occurred while removing default address.");
        }
    }

    public async Task<Result<PagedResult<UserAddress>>> GetAddressesByTypeAsync(PagedRequest request, Guid userId, short addressType,
        CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => {
            r.WithFilter("UserId", userId);
            r.WithFilter("AddressType", addressType);
        }, "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<bool>> ExistsAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        if (addressId == Guid.Empty) return Result<bool>.Failure("Invalid address id.");
        return await ExistsByCountAsync(UserAddressFields.AddressId, addressId, cancellationToken);
    }
}