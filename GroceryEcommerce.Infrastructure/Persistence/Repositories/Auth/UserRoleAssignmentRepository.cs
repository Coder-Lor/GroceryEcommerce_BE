using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using GroceryEcommerce.FactoryClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class UserRoleAssignmentRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<UserRoleAssignmentRepository> logger
) : BasePagedRepository<UserRoleAssignmentEntity, UserRoleAssignment>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IUserRoleAssignmentRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("RoleId", typeof(Guid)),
            new SearchableField("AssignedBy", typeof(Guid)),
            new SearchableField("AssignedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "AssignedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "RoleId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AssignedBy", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "AssignedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "UserId", UserRoleAssignmentFields.UserId },
            { "RoleId", UserRoleAssignmentFields.RoleId },
            { "AssignedBy", UserRoleAssignmentFields.AssignedBy },
            { "AssignedAt", UserRoleAssignmentFields.AssignedAt }
        };
    }

    protected override EntityQuery<UserRoleAssignmentEntity> ApplySearch(EntityQuery<UserRoleAssignmentEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        // try parse as Guid and search by ids
        if (Guid.TryParse(searchTerm.Trim(), out var guid))
        {
            return query.Where(
                UserRoleAssignmentFields.UserId == guid |
                UserRoleAssignmentFields.RoleId == guid |
                UserRoleAssignmentFields.AssignedBy == guid
            );
        }

        // no text search supported on this entity (role name requires join) -> return unchanged
        return query;
    }

    protected EntityField2? GetSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return null;
        return sortBy.ToLowerInvariant() switch
        {
            "userid" => UserRoleAssignmentFields.UserId,
            "roleid" => UserRoleAssignmentFields.RoleId,
            "assignedby" => UserRoleAssignmentFields.AssignedBy,
            _ => UserRoleAssignmentFields.AssignedAt
        };
    }

    protected override EntityQuery<UserRoleAssignmentEntity> ApplySorting(EntityQuery<UserRoleAssignmentEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;

        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<UserRoleAssignmentEntity> ApplyDefaultSorting(EntityQuery<UserRoleAssignmentEntity> query)
    {
        return query.OrderBy(UserRoleAssignmentFields.AssignedAt.Descending());
    }

    protected override async Task<IList<UserRoleAssignmentEntity>> FetchEntitiesAsync(EntityQuery<UserRoleAssignmentEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<UserRoleAssignmentEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public async Task<Result<UserRoleAssignment?>> GetByIdAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || roleId == Guid.Empty)
                return Result<UserRoleAssignment?>.Failure("Invalid identifiers.");

            var entity = new UserRoleAssignmentEntity(roleId, userId);
            var adapter = GetAdapter();
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            if (!fetched)
                return Result<UserRoleAssignment?>.Success(null);

            var domain = Mapper.Map<UserRoleAssignment>(entity);
            return Result<UserRoleAssignment?>.Success(domain);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting user role assignment: {UserId} {RoleId}", userId, roleId);
            return Result<UserRoleAssignment?>.Failure("An error occurred while retrieving role assignment.");
        }
    }

    public async Task<Result<PagedResult<UserRoleAssignment>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("UserId", userId), "AssignedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<PagedResult<UserRoleAssignment>>> GetByRoleIdAsync(PagedRequest request, Guid roleId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("RoleId", roleId), "AssignedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<UserRoleAssignment>> CreateAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<UserRoleAssignmentEntity>(assignment);
            // ensure PKs
            if (entity.RoleId == Guid.Empty) entity.RoleId = assignment.RoleId;
            if (entity.UserId == Guid.Empty) entity.UserId = assignment.UserId;
            entity.AssignedAt = DateTime.UtcNow;
            entity.IsNew = true;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                Logger.LogError("Failed to create user role assignment for user {UserId} role {RoleId}", entity.UserId, entity.RoleId);
                return Result<UserRoleAssignment>.Failure("Failed to create role assignment.");
            }

            // invalidate possible caches
            await CacheService.RemoveAsync($"UserRoles_ByUser_{entity.UserId}", cancellationToken);

            Logger.LogInformation("User role assignment created: {UserId} - {RoleId}", entity.UserId, entity.RoleId);
            return Result<UserRoleAssignment>.Success(Mapper.Map<UserRoleAssignment>(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating user role assignment: {UserId} {RoleId}", assignment.UserId, assignment.RoleId);
            return Result<UserRoleAssignment>.Failure("An error occurred while creating role assignment.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UserRoleAssignment assignment, CancellationToken cancellationToken = default)
    {
        try
        {
            if (assignment.UserId == Guid.Empty || assignment.RoleId == Guid.Empty)
                return Result<bool>.Failure("Invalid identifiers.");

            var entity = new UserRoleAssignmentEntity(assignment.RoleId, assignment.UserId);
            var adapter = GetAdapter();
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            if (!fetched)
                return Result<bool>.Failure("Role assignment not found.");

            Mapper.Map(assignment, entity);
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<bool>.Failure("Failed to update role assignment.");

            await CacheService.RemoveAsync($"UserRoles_ByUser_{entity.UserId}", cancellationToken);
            Logger.LogInformation("User role assignment updated: {UserId} - {RoleId}", entity.UserId, entity.RoleId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating user role assignment: {UserId} {RoleId}", assignment.UserId, assignment.RoleId);
            return Result<bool>.Failure("An error occurred while updating role assignment.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || roleId == Guid.Empty) return Result<bool>.Failure("Invalid identifiers.");

            var entity = new UserRoleAssignmentEntity(roleId, userId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (!deleted) return Result<bool>.Failure("Role assignment not found or failed to delete.");

            await CacheService.RemoveAsync($"UserRoles_ByUser_{userId}", cancellationToken);
            Logger.LogInformation("User role assignment deleted: {UserId} - {RoleId}", userId, roleId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting user role assignment: {UserId} {RoleId}", userId, roleId);
            return Result<bool>.Failure("An error occurred while deleting role assignment.");
        }
    }

    public async Task<Result<bool>> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid assignedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || roleId == Guid.Empty) return Result<bool>.Failure("Invalid identifiers.");

            var entity = new UserRoleAssignmentEntity(roleId, userId)
            {
                AssignedBy = assignedBy,
                AssignedAt = DateTime.UtcNow,
                IsNew = true
            };

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<bool>.Failure("Failed to assign role to user.");

            await CacheService.RemoveAsync($"UserRoles_ByUser_{userId}", cancellationToken);
            Logger.LogInformation("Assigned role {RoleId} to user {UserId} by {AssignedBy}", roleId, userId, assignedBy);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error assigning role to user: {UserId} {RoleId}", userId, roleId);
            return Result<bool>.Failure("An error occurred while assigning role.");
        }
    }

    public async Task<Result<bool>> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        => await DeleteAsync(userId, roleId, cancellationToken);

    public async Task<Result<bool>> HasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || roleId == Guid.Empty) return Result<bool>.Failure("Invalid identifiers.");

            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserRoleAssignmentFields.UserId == userId);
            filter.Add(UserRoleAssignmentFields.RoleId == roleId);
            bucket.PredicateExpression.Add(filter);

            var entity = await Task.Run(() => adapter.FetchNewEntity<UserRoleAssignmentEntity>(bucket), cancellationToken);
            return Result<bool>.Success(entity != null);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking role for user: {UserId} {RoleId}", userId, roleId);
            return Result<bool>.Failure("An error occurred while checking role.");
        }
    }

    public async Task<Result<bool>> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty || string.IsNullOrWhiteSpace(roleName)) return Result<bool>.Failure("Invalid input.");
            // find role
            var qf = new QueryFactory();
            var roleEntity = await GetAdapter().FetchFirstAsync(qf.UserRole.Where(UserRoleFields.RoleName == roleName), cancellationToken);
            if (roleEntity == null) return Result<bool>.Success(false);
            return await HasRoleAsync(userId, roleEntity.RoleId, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking role by name for user: {UserId} {RoleName}", userId, roleName);
            return Result<bool>.Failure("An error occurred while checking role.");
        }
    }

    public async Task<Result<List<string>>> GetUserRoleNamesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userRoleAssignment = new EntityCollection<UserRoleAssignmentEntity>();
            var prefetchPath = new PrefetchPath2(EntityType.UserRoleAssignmentEntity);
            prefetchPath.Add(UserRoleAssignmentEntity.PrefetchPathUserRole);

            var filter = new PredicateExpression(UserRoleAssignmentFields.UserId == userId);
            var queryParameter = new QueryParameters
            {
                CollectionToFetch = userRoleAssignment,
                FilterToUse = filter,
                PrefetchPathToUse = prefetchPath
            };

            var adapter = GetAdapter();
            await adapter.FetchEntityCollectionAsync(queryParameter, cancellationToken);

            var roleNames = userRoleAssignment
                .Select(ura => ura.UserRole?.RoleName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Select(name => name!)
                .ToList();

            Logger.LogInformation("Retrieved {Count} role names for user: {UserId}", roleNames.Count, userId);
            return Result<List<string>>.Success(roleNames);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting role names for user: {UserId}", userId);
            return Result<List<string>>.Failure("An error occurred while retrieving role names.");
        }
    }

    public async Task<Result<List<Guid>>> GetUserRoleIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var qf = new QueryFactory();
            var query = qf.UserRoleAssignment.Where(UserRoleAssignmentFields.UserId == userId);
            var adapter = GetAdapter();

            var entities = new EntityCollection<UserRoleAssignmentEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var ids = entities.Select(e => e.RoleId).ToList();

            return Result<List<Guid>>.Success(ids);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting role ids for user: {UserId}", userId);
            return Result<List<Guid>>.Failure("An error occurred while retrieving role ids.");
        }
    }

    public async Task<Result<List<Guid>>> GetUsersByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var qf = new QueryFactory();
            var query = qf.UserRoleAssignment.Where(UserRoleAssignmentFields.RoleId == roleId);
            var adapter = GetAdapter();

            var entities = new EntityCollection<UserRoleAssignmentEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var ids = entities.Select(e => e.UserId).ToList();

            return Result<List<Guid>>.Success(ids);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting users by role: {RoleId}", roleId);
            return Result<List<Guid>>.Failure("An error occurred while retrieving users for role.");
        }
    }

    public async Task<Result<List<Guid>>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleName)) return Result<List<Guid>>.Failure("Invalid role name.");
            var qf = new QueryFactory();
            var role = await GetAdapter().FetchFirstAsync(qf.UserRole.Where(UserRoleFields.RoleName == roleName), cancellationToken);
            if (role == null) return Result<List<Guid>>.Success(new List<Guid>());
            return await GetUsersByRoleAsync(role.RoleId, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting users by role name: {RoleName}", roleName);
            return Result<List<Guid>>.Failure("An error occurred while retrieving users for role.");
        }
    }

    public async Task<Result<bool>> RemoveAllRolesFromUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<bool>.Failure("Invalid user id.");

            var bucket = new RelationPredicateBucket(UserRoleAssignmentFields.UserId == userId);
            var entities = new EntityCollection<UserRoleAssignmentEntity>();
            var qp = new QueryParameters { CollectionToFetch = entities, FilterToUse = bucket.PredicateExpression };
            var adapter = GetAdapter();
            await adapter.FetchEntityCollectionAsync(qp, cancellationToken);

            if (entities.Count == 0) return Result<bool>.Success(true);

            await adapter.DeleteEntityCollectionAsync(entities, cancellationToken);
            Logger.LogInformation("Removed {Count} role assignments for user: {UserId}", entities.Count, userId);

            await CacheService.RemoveAsync($"UserRoles_ByUser_{userId}", cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error removing all roles from user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while removing roles.");
        }
    }
}