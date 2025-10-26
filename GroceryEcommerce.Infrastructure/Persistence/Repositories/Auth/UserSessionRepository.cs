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
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using SD.LLBLGen.Pro.QuerySpec;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class UserSessionRepository(DataAccessAdapter scopedAdapter, IUnitOfWorkService unitOfWorkService, IMapper mapper, ICacheService cacheService, ILogger<UserSessionRepository> logger) : BasePagedRepository<UserSessionEntity, UserSession>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IUserSessionRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "createdat" => UserSessionFields.CreatedAt,
            "expiresat" => UserSessionFields.ExpiresAt,
            "userid" => UserSessionFields.UserId,
            _ => UserSessionFields.CreatedAt
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("SessionId", typeof(Guid)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("SessionToken", typeof(string)),
            new SearchableField("DeviceInfo", typeof(string)),
            new SearchableField("IpAddress", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("ExpiresAt", typeof(DateTime)),
            new SearchableField("Revoked", typeof(bool))
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
            new FieldMapping { FieldName = "SessionId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "SessionToken", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "DeviceInfo", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "IpAddress", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ExpiresAt", FieldType = typeof(DateTime?), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Revoked", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            ["sessionid"] = UserSessionFields.SessionId,
            ["userid"] = UserSessionFields.UserId,
            ["sessiontoken"] = UserSessionFields.SessionToken,
            ["deviceinfo"] = UserSessionFields.DeviceInfo,
            ["ipaddress"] = UserSessionFields.IpAddress,
            ["createdat"] = UserSessionFields.CreatedAt,
            ["expiresat"] = UserSessionFields.ExpiresAt,
            ["revoked"] = UserSessionFields.Revoked
        };
    }

    protected override EntityQuery<UserSessionEntity> ApplySearch(EntityQuery<UserSessionEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim();
        // try GUID search
        if (Guid.TryParse(searchTerm, out var guid))
        {
            return query.Where(
                UserSessionFields.SessionId == guid |
                UserSessionFields.UserId == guid
            );
        }

        return query.Where(
            UserSessionFields.SessionToken.Contains(searchTerm) |
            UserSessionFields.DeviceInfo.Contains(searchTerm) |
            UserSessionFields.IpAddress.Contains(searchTerm)
        );
    }

    protected override EntityQuery<UserSessionEntity> ApplySorting(EntityQuery<UserSessionEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? UserSessionFields.CreatedAt;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<UserSessionEntity> ApplyDefaultSorting(EntityQuery<UserSessionEntity> query)
    {
        return query.OrderBy(UserSessionFields.CreatedAt.Descending());
    }

    protected override async Task<IList<UserSessionEntity>> FetchEntitiesAsync(EntityQuery<UserSessionEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<UserSessionEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public async Task<Result<UserSession?>> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (sessionId == Guid.Empty) return Result<UserSession?>.Failure("Invalid session id.");
            var entity = new UserSessionEntity(sessionId);
            var adapter = GetAdapter();
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            if (!fetched) return Result<UserSession?>.Success(null);
            return Result<UserSession?>.Success(Mapper.Map<UserSession>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching session by id: {SessionId}", sessionId);
            return Result<UserSession?>.Failure("An error occurred while fetching session.");
        }
    }

    public async Task<Result<UserSession?>> GetByTokenAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) return Result<UserSession?>.Failure("Session token is required.");
            var cacheKey = $"UserSession_ByToken_{sessionToken}";
            var cached = await CacheService.GetAsync<UserSession>(cacheKey, cancellationToken);
            if (cached != null) return Result<UserSession?>.Success(cached);

            var qf = new QueryFactory();
            var entity = await GetAdapter().FetchFirstAsync(qf.UserSession.Where(UserSessionFields.SessionToken == sessionToken), cancellationToken);
            if (entity == null) return Result<UserSession?>.Success(null);
            var domain = Mapper.Map<UserSession>(entity);
            await CacheService.SetAsync(cacheKey, domain, TimeSpan.FromMinutes(30), cancellationToken);
            return Result<UserSession?>.Success(domain);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching session by token");
            return Result<UserSession?>.Failure("An error occurred while fetching session by token.");
        }
    }

    public async Task<Result<PagedResult<UserSession>>> GetByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("UserId", userId), "CreatedAt", SortDirection.Descending, cancellationToken);

    public async Task<Result<UserSession>> CreateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<UserSessionEntity>(session);
            if (entity.SessionId == Guid.Empty) entity.SessionId = session.SessionId == Guid.Empty ? Guid.NewGuid() : session.SessionId;
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsNew = true;

            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved) return Result<UserSession>.Failure("Failed to create session.");

            await CacheService.RemoveAsync($"UserSessions_ByUser_{entity.UserId}", cancellationToken);
            await CacheService.RemoveAsync($"UserSession_ByToken_{entity.SessionToken}", cancellationToken);
            return Result<UserSession>.Success(Mapper.Map<UserSession>(entity));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating session");
            return Result<UserSession>.Failure("An error occurred while creating session.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        try
        {
            if (session.SessionId == Guid.Empty) return Result<bool>.Failure("Invalid session id.");
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(new QueryFactory().UserSession.Where(UserSessionFields.SessionId == session.SessionId), cancellationToken);
            if (entity == null) return Result<bool>.Failure("Session not found.");

            Mapper.Map(session, entity);
            entity.IsNew = false;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync($"UserSessions_ByUser_{entity.UserId}", cancellationToken);
                await CacheService.RemoveAsync($"UserSession_ByToken_{entity.SessionToken}", cancellationToken);
            }
            return Result<bool>.Success(saved);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating session: {SessionId}", session.SessionId);
            return Result<bool>.Failure("An error occurred while updating session.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (sessionId == Guid.Empty) return Result<bool>.Failure("Invalid session id.");
            var entity = new UserSessionEntity(sessionId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted)
            {
                await CacheService.RemoveAsync($"UserSessions_ByUser_{entity.UserId}", cancellationToken);
                await CacheService.RemoveAsync($"UserSession_ByToken_{entity.SessionToken}", cancellationToken);
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure("Session not found or not deleted.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting session: {SessionId}", sessionId);
            return Result<bool>.Failure("An error occurred while deleting session.");
        }
    }

    public async Task<Result<bool>> RevokeSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (sessionId == Guid.Empty) return Result<bool>.Failure("Invalid session id.");
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(new QueryFactory().UserSession.Where(UserSessionFields.SessionId == sessionId), cancellationToken);
            if (entity == null) return Result<bool>.Failure("Session not found.");
            entity.Revoked = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync($"UserSessions_ByUser_{entity.UserId}", cancellationToken);
                await CacheService.RemoveAsync($"UserSession_ByToken_{entity.SessionToken}", cancellationToken);
                return Result<bool>.Success(true);
            }
            return Result<bool>.Failure("Failed to revoke session.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking session: {SessionId}", sessionId);
            return Result<bool>.Failure("An error occurred while revoking session.");
        }
    }

    public async Task<Result<bool>> RevokeAllUserSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<bool>.Failure("Invalid user id.");
            var adapter = GetAdapter();
            var bucket = new RelationPredicateBucket(UserSessionFields.UserId == userId);
            var entities = new EntityCollection<UserSessionEntity>();
            var qp = new QueryParameters { CollectionToFetch = entities, FilterToUse = bucket.PredicateExpression };
            await adapter.FetchEntityCollectionAsync(qp, cancellationToken);
            if (entities.Count == 0) return Result<bool>.Success(true);

            foreach (var e in entities)
            {
                e.Revoked = true;
            }

            await adapter.SaveEntityCollectionAsync(entities, cancellationToken);
            await CacheService.RemoveAsync($"UserSessions_ByUser_{userId}", cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking all sessions for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while revoking sessions.");
        }
    }

    public async Task<Result<bool>> RevokeExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;
            var bucket = new RelationPredicateBucket();
            bucket.PredicateExpression.Add(UserSessionFields.ExpiresAt < now);
            bucket.PredicateExpression.Add(UserSessionFields.Revoked == false);

            var entities = new EntityCollection<UserSessionEntity>();
            var qp = new QueryParameters { CollectionToFetch = entities, FilterToUse = bucket.PredicateExpression };
            var adapter = GetAdapter();
            await adapter.FetchEntityCollectionAsync(qp, cancellationToken);
            if (entities.Count == 0) return Result<bool>.Success(true);

            foreach (var e in entities)
            {
                e.Revoked = true;
            }

            await adapter.SaveEntityCollectionAsync(entities, cancellationToken);
            // Invalidate any per-user caches touched
            var userIds = entities.Select(x => x.UserId).Distinct().Where(id => id != null).Select(id => id!.Value);
            foreach (var uid in userIds)
            {
                await CacheService.RemoveAsync($"UserSessions_ByUser_{uid}", cancellationToken);
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking expired sessions");
            return Result<bool>.Failure("An error occurred while revoking expired sessions.");
        }
    }

    public async Task<Result<PagedResult<UserSession>>> GetActiveSessionsByUserIdAsync(PagedRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<PagedResult<UserSession>>.Failure("Invalid user id.");

            // Validate request similar to BasePagedRepository.GetPagedAsync
            request.AvailableFields = GetSearchableFields();
            var validation = request.Validate();
            if (validation != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                logger.LogWarning("Invalid paged request: {Errors}", validation?.ErrorMessage);
                return Result<PagedResult<UserSession>>.Failure(validation?.ErrorMessage ?? "Invalid paged request");
            }

            var qf = new QueryFactory();

            
            var now = DateTime.UtcNow;
            var baseQuery = qf.UserSession
                .Where(UserSessionFields.UserId == userId)
                .Where(UserSessionFields.Revoked == false)
                .Where(UserSessionFields.ExpiresAt.IsNull().Or(UserSessionFields.ExpiresAt > now));

            // total count
            var adapter = GetAdapter();
            var total = await adapter.FetchScalarAsync<int>(baseQuery.Select(() => Functions.CountRow()), cancellationToken);

            // apply sorting
            if (request.HasSorting)
            {
                baseQuery = ApplySorting(baseQuery, request.SortBy, request.SortDirection);
            }
            else
            {
                baseQuery = ApplyDefaultSorting(baseQuery);
            }

            // apply paging
            baseQuery = baseQuery.Page(request.Page, request.PageSize);

            // fetch entities
            var entities = await FetchEntitiesAsync(baseQuery, adapter, cancellationToken);
            var domain = Mapper.Map<List<UserSession>>(entities);

            var result = new PagedResult<UserSession>(domain, total, request.Page, request.PageSize);
            return Result<PagedResult<UserSession>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active sessions for user: {UserId}", userId);
            return Result<PagedResult<UserSession>>.Failure("An error occurred while getting active sessions.");
        }
    }

    public async Task<Result<bool>> IsSessionValidAsync(string sessionToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sessionToken)) return Result<bool>.Failure("Session token is required.");
            var qf = new QueryFactory();
            var now = DateTime.UtcNow;

            var countQuery = qf.UserSession
                .Where(UserSessionFields.SessionToken == sessionToken)
                .Where(UserSessionFields.Revoked == false)
                .Where(UserSessionFields.ExpiresAt.IsNull().Or(UserSessionFields.ExpiresAt > now));

            var entity = await GetAdapter().FetchFirstAsync(countQuery, cancellationToken);
            return Result<bool>.Success(entity != null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating session token");
            return Result<bool>.Failure("An error occurred while validating session.");
        }
    }

    public async Task<Result<int>> GetActiveSessionCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (userId == Guid.Empty) return Result<int>.Failure("Invalid user id.");
            var qf = new QueryFactory();
            var now = DateTime.UtcNow;

            var countQuery = qf.UserSession
                .Where(UserSessionFields.UserId == userId)
                .Where(UserSessionFields.Revoked == false)
                .Where(UserSessionFields.ExpiresAt.IsNull().Or(UserSessionFields.ExpiresAt > now))
                .Select(() => Functions.CountRow());

            var total = await GetAdapter().FetchScalarAsync<int>(countQuery, cancellationToken);
            return Result<int>.Success(total);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active session count for user: {UserId}", userId);
            return Result<int>.Failure("An error occurred while retrieving active session count.");
        }
    }
}