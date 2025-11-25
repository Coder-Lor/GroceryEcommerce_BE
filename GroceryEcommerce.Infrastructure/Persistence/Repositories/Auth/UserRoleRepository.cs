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
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class UserRoleRepository(DataAccessAdapter scopedAdapter, IUnitOfWorkService unitOfWorkService, IMapper mapper, ICacheService cacheService, ILogger<UserRoleRepository> logger) : BasePagedRepository<UserRoleEntity, UserRole>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IUserRoleRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "rolename" => UserRoleFields.RoleName,
            "roleid" => UserRoleFields.RoleId,
            "createdat" => UserRoleFields.CreatedAt,
            _ => UserRoleFields.RoleName
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("RoleName", typeof(string)),
            new SearchableField("RoleId", typeof(Guid)),
            new SearchableField("Description", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime))
        };
    }

    public override string? GetDefaultSortField()
    {
        return "RoleName";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "RoleName", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "RoleId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Description", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            ["rolename"] = UserRoleFields.RoleName,
            ["roleid"] = UserRoleFields.RoleId,
            ["description"] = UserRoleFields.Description,
            ["createdat"] = UserRoleFields.CreatedAt
        };
    }

    protected override EntityQuery<UserRoleEntity> ApplySearch(EntityQuery<UserRoleEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim();

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            UserRoleFields.RoleName,
            UserRoleFields.Description,
            UserRoleFields.RoleId);

        return query.Where(predicate);
    }

    protected override EntityQuery<UserRoleEntity> ApplySorting(EntityQuery<UserRoleEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? UserRoleFields.RoleName;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<UserRoleEntity> ApplyDefaultSorting(EntityQuery<UserRoleEntity> query)
    {
        return query.OrderBy(UserRoleFields.RoleName.Ascending());
    }

    protected override async Task<IList<UserRoleEntity>> FetchEntitiesAsync(EntityQuery<UserRoleEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<UserRoleEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return UserRoleFields.RoleId;
    }

    protected override object GetEntityId(UserRoleEntity entity, EntityField2 primaryKeyField)
    {
        return entity.RoleId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public Task<Result<UserRole?>> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        if (roleId == Guid.Empty)
        {
            logger.LogWarning("Role id is required");
            return Task.FromResult(Result<UserRole?>.Failure("Invalid role ID."));
        }
        return GetSingleAsync(UserRoleFields.RoleId, roleId, "UserRole", TimeSpan.FromHours(1), cancellationToken);
    }

    public Task<Result<UserRole?>> GetByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            logger.LogWarning("Role name is required");
            return Task.FromResult(Result<UserRole?>.Failure("Invalid role name."));
        }
        return GetSingleAsync(UserRoleFields.RoleName, roleName, "UserRole_ByName", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<List<UserRole>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = "All_UserRoles";
            var cached = await CacheService.GetAsync<List<UserRole>>(cacheKey, cancellationToken);
            if (cached != null) return Result<List<UserRole>>.Success(cached);

            var qf = new QueryFactory();
            var query = qf.UserRole.OrderBy(UserRoleFields.RoleName.Ascending());
            var adapter = GetAdapter();
            var entities = new EntityCollection<UserRoleEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var list = entities.Select(e => Mapper.Map<UserRole>(e)).ToList();
            await CacheService.SetAsync(cacheKey, list, TimeSpan.FromHours(1), cancellationToken);
            return Result<List<UserRole>>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all roles");
            return Result<List<UserRole>>.Failure("An error occurred while retrieving roles.");
        }
    }

    public async Task<Result<UserRole>> CreateAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<UserRoleEntity>(role);
            if (entity.RoleId == Guid.Empty) entity.RoleId = role.RoleId == Guid.Empty ? Guid.NewGuid() : role.RoleId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsNew = true;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                logger.LogError("Failed to create role: {RoleName}", role.RoleName);
                return Result<UserRole>.Failure("Failed to create role.");
            }

            await CacheService.RemoveAsync("All_UserRoles", cancellationToken);
            await CacheService.RemoveAsync($"UserRole_{entity.RoleId}", cancellationToken);
            await CacheService.RemoveAsync($"UserRole_ByName_{entity.RoleName}", cancellationToken);

            logger.LogInformation("Role created: {RoleId} - {RoleName}", entity.RoleId, entity.RoleName);
            return Result<UserRole>.Success(Mapper.Map<UserRole>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating role: {RoleName}", role.RoleName);
            return Result<UserRole>.Failure("An error occurred while creating role.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        try
        {
            if (role.RoleId == Guid.Empty)
                return Result<bool>.Failure("Invalid role ID.");

            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(new QueryFactory().UserRole.Where(UserRoleFields.RoleId == role.RoleId), cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Role not found: {RoleId}", role.RoleId);
                return Result<bool>.Failure("Role not found.");
            }

            Mapper.Map(role, entity);
            entity.IsNew = false;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync("All_UserRoles", cancellationToken);
                await CacheService.RemoveAsync($"UserRole_{entity.RoleId}", cancellationToken);
                await CacheService.RemoveAsync($"UserRole_ByName_{entity.RoleName}", cancellationToken);
            }

            logger.LogInformation("Role updated: {RoleId}", role.RoleId);
            return Result<bool>.Success(saved);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating role: {RoleId}", role.RoleId);
            return Result<bool>.Failure("An error occurred while updating role.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (roleId == Guid.Empty)
                return Result<bool>.Failure("Invalid role ID.");

            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(new QueryFactory().UserRole.Where(UserRoleFields.RoleId == roleId), cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Role not found: {RoleId}", roleId);
                return Result<bool>.Failure("Role not found.");
            }

            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted)
            {
                await CacheService.RemoveAsync("All_UserRoles", cancellationToken);
                await CacheService.RemoveAsync($"UserRole_{roleId}", cancellationToken);
                await CacheService.RemoveAsync($"UserRole_ByName_{entity.RoleName}", cancellationToken);
                logger.LogInformation("Role deleted: {RoleId}", roleId);
                return Result<bool>.Success(true);
            }

            logger.LogWarning("Role not deleted: {RoleId}", roleId);
            return Result<bool>.Failure("Role not deleted.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting role: {RoleId}", roleId);
            return Result<bool>.Failure("An error occurred while deleting role.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            logger.LogWarning("Role name is required");
            return Result<bool>.Failure("Invalid role name.");
        }
        return await ExistsByCountAsync(UserRoleFields.RoleName, roleName, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        if (roleId == Guid.Empty)
        {
            logger.LogWarning("Role id is required");
            return Result<bool>.Failure("Invalid role ID.");
        }
        return await ExistsByCountAsync(UserRoleFields.RoleId, roleId, cancellationToken);
    }

    public async Task<Result<List<UserRole>>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // assuming active roles are all roles; adjust if there is an "IsActive" field
            var qf = new QueryFactory();
            var query = qf.UserRole.OrderBy(UserRoleFields.RoleName.Ascending());
            var adapter = GetAdapter();
            var entities = new EntityCollection<UserRoleEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var list = entities.Select(e => Mapper.Map<UserRole>(e)).ToList();
            return Result<List<UserRole>>.Success(list);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active roles");
            return Result<List<UserRole>>.Failure("An error occurred while retrieving active roles.");
        }
    }

    public async Task<Result<bool>> IsRoleInUseAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (roleId == Guid.Empty) return Result<bool>.Failure("Invalid role ID.");
            // check assignments
            return await ExistsByCountAsync(UserRoleAssignmentFields.RoleId, roleId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if role is in use: {RoleId}", roleId);
            return Result<bool>.Failure("An error occurred while checking role usage.");
        }
    }
}