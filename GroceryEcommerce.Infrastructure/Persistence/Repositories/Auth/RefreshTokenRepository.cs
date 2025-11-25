using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth;

public class RefreshTokenRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<RefreshTokenRepository> logger
): BasePagedRepository<RefreshTokenEntity, RefreshToken>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IRefreshTokenRepository
{
    public async Task<Result<RefreshToken?>> GetByIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.TokenId == tokenId);
            
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync<RefreshTokenEntity>(query, cancellationToken);
            if (entity == null) {
                Logger.LogWarning("Refresh token not found by id: {TokenId}", tokenId);
                return Result<RefreshToken?>.Success(null);
            }
            return Result<RefreshToken?>.Success(Mapper.Map<RefreshToken>(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting refresh token by id: {TokenId}", tokenId);
            return Result<RefreshToken?>.Failure("An error occurred while retrieving refresh token", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<RefreshToken?>> GetByTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.RefreshToken == refreshTokenValue);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync<RefreshTokenEntity>(query, cancellationToken);
            
            if (entity == null) {
                Logger.LogWarning("Refresh token not found by token value");
                return Result<RefreshToken?>.Success(null);
            }
            return Result<RefreshToken?>.Success(Mapper.Map<RefreshToken>(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting refresh token by token value");
            return Result<RefreshToken?>.Failure("An error occurred while retrieving refresh token", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<List<RefreshToken>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.UserId == userId);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            
            var refreshTokens = Mapper.Map<List<RefreshToken>>(entities);
            Logger.LogInformation("Retrieved {Count} refresh tokens for user: {UserId}", refreshTokens.Count, userId);
            
            return Result<List<RefreshToken>>.Success(refreshTokens);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting refresh tokens by user id: {UserId}", userId);
            return Result<List<RefreshToken>>.Failure("An error occurred while retrieving refresh tokens", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<List<RefreshToken>>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.UserId == userId)
            .Where(RefreshTokenFields.Revoked == false)
            .Where(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            
            Logger.LogInformation("Retrieved {Count} active refresh tokens for user: {UserId}", entities.Count, userId);
            
            return Result<List<RefreshToken>>.Success(Mapper.Map<List<RefreshToken>>(entities));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting active refresh tokens by user id: {UserId}", userId);
            return Result<List<RefreshToken>>.Failure("An error occurred while retrieving active refresh tokens", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<RefreshToken>> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<RefreshTokenEntity>(refreshToken);
            
            // Ensure TokenId is set if not already provided
            if (entity.TokenId == Guid.Empty)
            {
                entity.TokenId = Guid.NewGuid();
            }
            
            // Ensure CreatedAt is set if not already provided
            if (entity.CreatedAt == DateTime.MinValue)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            
            entity.IsNew = true;
            var adapter = GetAdapter();
            await adapter.SaveEntityAsync(entity, true, cancellationToken);

            return Result<RefreshToken>.Success(Mapper.Map<RefreshToken>(entity));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating refresh token for user: {UserId}", refreshToken.UserId);
            return Result<RefreshToken>.Failure("An error occurred while creating refresh token", "REFRESH_TOKEN_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.TokenId == refreshToken.TokenId);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync<RefreshTokenEntity>(query, cancellationToken);
            
            if (entity == null)
            {
                Logger.LogWarning("Refresh token not found for update: {TokenId}", refreshToken.TokenId);
                return Result<bool>.Failure("Refresh token not found", "REFRESH_TOKEN_UPDATE_001");
            }
            
            entity.RefreshToken = refreshToken.RefreshTokenValue;
            entity.ExpiresAt = refreshToken.ExpiresAt;
            entity.Revoked = refreshToken.Revoked;
            entity.CreatedAt = refreshToken.CreatedAt;
            entity.CreatedByIp = refreshToken.CreatedByIp;
            entity.ReplacedByToken = refreshToken.ReplacedByToken;
            entity.IsNew = false;
            entity.IsDirty = true;

            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            
            if (!saved)
            {
                Logger.LogError("Failed to update refresh token: {TokenId}", refreshToken.TokenId);
                return Result<bool>.Failure("Failed to update refresh token", "REFRESH_TOKEN_UPDATE_001");
            }
            
            Logger.LogInformation("Refresh token updated successfully: {TokenId}", refreshToken.TokenId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating refresh token: {TokenId}", refreshToken.TokenId);
            return Result<bool>.Failure("An error occurred while updating refresh token", "REFRESH_TOKEN_UPDATE_ERROR");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.TokenId == tokenId);


            var adapter = GetAdapter(); // Sử dụng adapter phù hợp

            var entity = await adapter.FetchFirstAsync<RefreshTokenEntity>(query, cancellationToken);
            if (entity != null)
            {
                await adapter.DeleteEntityAsync(entity, cancellationToken);
                Logger.LogInformation("Refresh token deleted successfully: {TokenId}", tokenId);
                return Result<bool>.Success(true);
            }
            Logger.LogWarning("Refresh token not found for deletion: {TokenId}", tokenId);
            return Result<bool>.Failure("Refresh token not found", "REFRESH_TOKEN_DELETE_001");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting refresh token: {TokenId}", tokenId);
            return Result<bool>.Failure("An error occurred while deleting refresh token", "REFRESH_TOKEN_DELETE_ERROR");
        }
    }

    public async Task<Result<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.TokenId == tokenId);
            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync<RefreshTokenEntity>(query, cancellationToken);
            if (entity == null)
            {
                Logger.LogWarning("Refresh token not found for revocation: {TokenId}", tokenId);
                return Result<bool>.Failure("Refresh token not found", "REFRESH_TOKEN_REVOKE_001");
            }
            entity.Revoked = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (!saved)
            {
                Logger.LogError("Failed to revoke refresh token: {TokenId}", tokenId);
                return Result<bool>.Failure("Failed to revoke refresh token", "REFRESH_TOKEN_REVOKE_002");
            }
            
            Logger.LogInformation("Refresh token revoked successfully: {TokenId}", tokenId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error revoking refresh token: {TokenId}", tokenId);
            return Result<bool>.Failure("An error occurred while revoking refresh token", "REFRESH_TOKEN_REVOKE_ERROR");
        }
    }

    public async Task<Result<bool>> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try {
            var query = new QueryFactory().RefreshToken
            .Where(RefreshTokenFields.UserId == userId)
            .Where(RefreshTokenFields.Revoked == false);
            var adapter = GetAdapter();
            var entities = await adapter.FetchQueryAsync(query, cancellationToken) as EntityCollection<RefreshTokenEntity>;
            if (entities == null || entities.Count == 0)
            {
                Logger.LogInformation("No active refresh tokens found for user: {UserId}", userId);
                return Result<bool>.Success(true);
            }

            foreach (var entity in entities)
            {
                entity.Revoked = true;
            }

            await adapter.SaveEntityCollectionAsync(entities, cancellationToken);
            Logger.LogInformation("All refresh tokens revoked successfully for user: {UserId}", userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error revoking all refresh tokens for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while revoking all refresh tokens", "REFRESH_TOKEN_REVOKE_ALL_ERROR");
        }
    }

    public async Task<Result<bool>> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.ExpiresAt < DateTime.UtcNow);
            bucket.PredicateExpression.Add(filter);
            var entities = new EntityCollection<RefreshTokenEntity>();

            var query = new QueryParameters
            {
                CollectionToFetch = entities,
                FilterToUse = bucket.PredicateExpression,
                PrefetchPathToUse = new PrefetchPath2(EntityType.RefreshTokenEntity)
            };
            
            var adapter = GetAdapter();
            await adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            await adapter.DeleteEntityCollectionAsync(entities, cancellationToken);
            
            Logger.LogInformation("Cleaned up {Count} expired refresh tokens", entities.Count);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error cleaning up expired refresh tokens");
            return Result<bool>.Failure("An error occurred while cleaning up expired refresh tokens", "REFRESH_TOKEN_CLEANUP_ERROR");
        }
    }

    public async Task<Result<bool>> IsTokenValidAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.RefreshToken == refreshTokenValue);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);
            bucket.PredicateExpression.Add(filter);
            
            var adapter = GetAdapter();
            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket), cancellationToken);
            
            var isValid = entity != null;
            Logger.LogInformation("Token validation result: {IsValid}", isValid);
            
            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating refresh token");
            return Result<bool>.Failure("An error occurred while validating refresh token", "REFRESH_TOKEN_VALIDATE_ERROR");
        }
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("TokenId", typeof(Guid)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("RefreshToken", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("ExpiresAt", typeof(DateTime)),
            new SearchableField("Revoked", typeof(bool))
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
            new FieldMapping { FieldName = "TokenId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "RefreshToken", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ExpiresAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Revoked", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "TokenId", RefreshTokenFields.TokenId },
            { "UserId", RefreshTokenFields.UserId },
            { "RefreshToken", RefreshTokenFields.RefreshToken },
            { "CreatedAt", RefreshTokenFields.CreatedAt },
            { "ExpiresAt", RefreshTokenFields.ExpiresAt },
            { "Revoked", RefreshTokenFields.Revoked }
        };
    }

    protected override EntityQuery<RefreshTokenEntity> ApplySearch(EntityQuery<RefreshTokenEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        var term = searchTerm.Trim();
        
        // Try parse as Guid first
        if (Guid.TryParse(term, out var guid))
        {
            return query.Where(
                RefreshTokenFields.TokenId == guid |
                RefreshTokenFields.UserId == guid
            );
        }
        
        // Text search on RefreshToken field
        return query.Where(SearchPredicateBuilder.BuildContainsPredicate(term, RefreshTokenFields.RefreshToken));
    }

    protected override EntityQuery<RefreshTokenEntity> ApplySorting(EntityQuery<RefreshTokenEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;

        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<RefreshTokenEntity> ApplyDefaultSorting(EntityQuery<RefreshTokenEntity> query)
    {
        return query.OrderBy(RefreshTokenFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return RefreshTokenFields.TokenId;
    }

    protected override object GetEntityId(RefreshTokenEntity entity, EntityField2 primaryKeyField)
    {
        return entity.TokenId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "tokenid" => RefreshTokenFields.TokenId,
            "userid" => RefreshTokenFields.UserId,
            "createdat" => RefreshTokenFields.CreatedAt,
            "expiresat" => RefreshTokenFields.ExpiresAt,
            "revoked" => RefreshTokenFields.Revoked,
            _ => RefreshTokenFields.CreatedAt
        };
    }

    protected override async Task<IList<RefreshTokenEntity>> FetchEntitiesAsync(EntityQuery<RefreshTokenEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<RefreshTokenEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }
}
