using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(
    IDataAccessAdapterFactory adapterFactory,
    IMapper mapper,
    ILogger<RefreshTokenRepository> logger
) : IRefreshTokenRepository
{
    public async Task<Result<RefreshToken?>> GetByIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var entity = new RefreshTokenEntity(tokenId);
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                logger.LogWarning("Refresh token not found by id: {TokenId}", tokenId);
                return Result<RefreshToken?>.Success(null);
            }
            
            var refreshToken = mapper.Map<RefreshToken>(entity);
            return Result<RefreshToken?>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting refresh token by id: {TokenId}", tokenId);
            return Result<RefreshToken?>.Failure("An error occurred while retrieving refresh token", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<RefreshToken?>> GetByTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket(RefreshTokenFields.RefreshToken == refreshTokenValue);
            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket), cancellationToken);
            
            if (entity == null)
            {
                logger.LogWarning("Refresh token not found by token value");
                return Result<RefreshToken?>.Success(null);
            }
            
            var refreshToken = mapper.Map<RefreshToken>(entity);
            return Result<RefreshToken?>.Success(refreshToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting refresh token by token value");
            return Result<RefreshToken?>.Failure("An error occurred while retrieving refresh token", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<List<RefreshToken>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket(RefreshTokenFields.UserId == userId);
            var entities = new EntityCollection<RefreshTokenEntity>();

            var query = new QueryParameters
            {
                CollectionToFetch = entities,
                FilterToUse = bucket.PredicateExpression,
                PrefetchPathToUse = new PrefetchPath2(EntityType.RefreshTokenEntity)
            };
            
            await adapter.FetchEntityCollectionAsync(query,cancellationToken);
            
            var refreshTokens = mapper.Map<List<RefreshToken>>(entities);
            logger.LogInformation("Retrieved {Count} refresh tokens for user: {UserId}", refreshTokens.Count, userId);
            
            return Result<List<RefreshToken>>.Success(refreshTokens);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting refresh tokens by user id: {UserId}", userId);
            return Result<List<RefreshToken>>.Failure("An error occurred while retrieving refresh tokens", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<List<RefreshToken>>> GetActiveTokensByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);
            bucket.PredicateExpression.Add(filter);
            
            var entities = new EntityCollection<RefreshTokenEntity>();

            var query = new QueryParameters
            {
                CollectionToFetch = entities,
                FilterToUse = bucket.PredicateExpression,
                PrefetchPathToUse = new PrefetchPath2(EntityType.RefreshTokenEntity)
            };
             await adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            var refreshTokens = mapper.Map<List<RefreshToken>>(entities);
            logger.LogInformation("Retrieved {Count} active refresh tokens for user: {UserId}", refreshTokens.Count, userId);
            
            return Result<List<RefreshToken>>.Success(refreshTokens);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active refresh tokens by user id: {UserId}", userId);
            return Result<List<RefreshToken>>.Failure("An error occurred while retrieving active refresh tokens", "REFRESH_TOKEN_GET_ERROR");
        }
    }

    public async Task<Result<RefreshToken>> CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var entity = mapper.Map<RefreshTokenEntity>(refreshToken);
            entity.TokenId = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            
            if (!saved)
            {
                logger.LogError("Failed to create refresh token for user: {UserId}", refreshToken.UserId);
                return Result<RefreshToken>.Failure("Failed to create refresh token", "REFRESH_TOKEN_CREATE_001");
            }
            
            var createdToken = mapper.Map<RefreshToken>(entity);
            logger.LogInformation("Refresh token created successfully: {TokenId}", entity.TokenId);
            
            return Result<RefreshToken>.Success(createdToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating refresh token for user: {UserId}", refreshToken.UserId);
            return Result<RefreshToken>.Failure("An error occurred while creating refresh token", "REFRESH_TOKEN_CREATE_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var entity = new RefreshTokenEntity(refreshToken.TokenId);
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                logger.LogWarning("Refresh token not found for update: {TokenId}", refreshToken.TokenId);
                return Result<bool>.Failure("Refresh token not found", "REFRESH_TOKEN_UPDATE_001");
            }
            
            // Map updated fields
            mapper.Map(refreshToken, entity);
            
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            
            if (!saved)
            {
                logger.LogError("Failed to update refresh token: {TokenId}", refreshToken.TokenId);
                return Result<bool>.Failure("Failed to update refresh token", "REFRESH_TOKEN_UPDATE_002");
            }
            
            logger.LogInformation("Refresh token updated successfully: {TokenId}", refreshToken.TokenId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating refresh token: {TokenId}", refreshToken.TokenId);
            return Result<bool>.Failure("An error occurred while updating refresh token", "REFRESH_TOKEN_UPDATE_ERROR");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var entity = new RefreshTokenEntity(tokenId);
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            
            if (!deleted)
            {
                logger.LogWarning("Failed to delete refresh token: {TokenId}", tokenId);
                return Result<bool>.Failure("Refresh token not found or already deleted", "REFRESH_TOKEN_DELETE_001");
            }
            
            logger.LogInformation("Refresh token deleted successfully: {TokenId}", tokenId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting refresh token: {TokenId}", tokenId);
            return Result<bool>.Failure("An error occurred while deleting refresh token", "REFRESH_TOKEN_DELETE_ERROR");
        }
    }

    public async Task<Result<bool>> RevokeTokenAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var entity = new RefreshTokenEntity(tokenId);
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);
            
            if (!fetched)
            {
                logger.LogWarning("Refresh token not found for revocation: {TokenId}", tokenId);
                return Result<bool>.Failure("Refresh token not found", "REFRESH_TOKEN_REVOKE_001");
            }
            
            entity.Revoked = true;
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            
            if (!saved)
            {
                logger.LogError("Failed to revoke refresh token: {TokenId}", tokenId);
                return Result<bool>.Failure("Failed to revoke refresh token", "REFRESH_TOKEN_REVOKE_002");
            }
            
            logger.LogInformation("Refresh token revoked successfully: {TokenId}", tokenId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking refresh token: {TokenId}", tokenId);
            return Result<bool>.Failure("An error occurred while revoking refresh token", "REFRESH_TOKEN_REVOKE_ERROR");
        }
    }

    public async Task<Result<bool>> RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            bucket.PredicateExpression.Add(filter);
            
            var entities = new EntityCollection<RefreshTokenEntity>();

            var query = new QueryParameters
            {
                CollectionToFetch = entities,
                FilterToUse = bucket.PredicateExpression,
                PrefetchPathToUse = new PrefetchPath2(EntityType.RefreshTokenEntity)
            };
            
            await adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            foreach (var entity in entities)
            {
                entity.Revoked = true;
            }
            
            var saved = await adapter.SaveEntityCollectionAsync(entities, cancellationToken);
            
            if (saved == 0)
            {
                logger.LogError("Failed to revoke all refresh tokens for user: {UserId}", userId);
                return Result<bool>.Failure("Failed to revoke all refresh tokens", "REFRESH_TOKEN_REVOKE_ALL_001");
            }
            
            logger.LogInformation("All refresh tokens revoked for user: {UserId}", userId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error revoking all refresh tokens for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while revoking all refresh tokens", "REFRESH_TOKEN_REVOKE_ALL_ERROR");
        }
    }

    public async Task<Result<bool>> CleanupExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
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
            
            await adapter.FetchEntityCollectionAsync(query, cancellationToken);
            
            var deleted = await adapter.DeleteEntityCollectionAsync(entities, cancellationToken);
            
            logger.LogInformation("Cleaned up {Count} expired refresh tokens", entities.Count);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cleaning up expired refresh tokens");
            return Result<bool>.Failure("An error occurred while cleaning up expired refresh tokens", "REFRESH_TOKEN_CLEANUP_ERROR");
        }
    }

    public async Task<Result<bool>> IsTokenValidAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.RefreshToken == refreshTokenValue);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);
            bucket.PredicateExpression.Add(filter);
            
            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket), cancellationToken);
            
            var isValid = entity != null;
            logger.LogInformation("Token validation result: {IsValid}", isValid);
            
            return Result<bool>.Success(isValid);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating refresh token");
            return Result<bool>.Failure("An error occurred while validating refresh token", "REFRESH_TOKEN_VALIDATE_ERROR");
        }
    }
}
