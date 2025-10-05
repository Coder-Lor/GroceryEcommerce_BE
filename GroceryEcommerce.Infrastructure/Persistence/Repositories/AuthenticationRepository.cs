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
using SD.LLBLGen.Pro.QuerySpec;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories;

public class AuthenticationRepository(
    IDataAccessAdapterFactory adapterFactory,
    IPasswordHashService passwordHashService,
    IMapper mapper,
    ILogger<AuthenticationRepository> logger
) : IAuthenticationRepository
{
    public async Task<Result<User?>> ValidateUserCredentialsAsync(string emailOrUsername, string password,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

            var bucket= new RelationPredicateBucket(
                UserFields.Email == emailOrUsername    
            );
            bucket.PredicateExpression.AddWithOr(UserFields.Username == emailOrUsername);

            var userResult = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

            if (userResult == null) {
                logger.LogWarning("User not found {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Failure("Invalid credentials", "AUTH_001");
            }

            if (!passwordHashService.VerifyPassword(password, userResult.PasswordHash)) {
                logger.LogWarning("Invalid password for user: {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Failure("Invalid credentials", "AUTH_002");
            }

            if(userResult.Status != 1) {
                logger.LogWarning("User account is not active: {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Failure("Account is inactive or banned", "AUTH_003");
            }

            var user = mapper.Map<User>(userResult);
            return Result<User?>.Success(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating user credentials for {EmailOrUsername}", emailOrUsername);
            return Result<User?>.Failure("An error occurred while validating credentials", "AUTH_ERROR");
        }
    }

    public async Task<Result<bool>> CreateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry,
        CancellationToken cancellationToken = default)
    {

        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var entity = new RefreshTokenEntity {
                TokenId = Guid.NewGuid(),
                UserId = userId,
                RefreshToken = refreshToken,
                ExpiresAt = expiry,
                CreatedAt = DateTime.UtcNow,
                Revoked = false,
            };

            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);

            if(!saved) {
                logger.LogError("Failed to create refresh token for user: {UserId}", userId);
                return Result<bool>.Failure("Failed to create refresh token", "TOKEN_001");
            }

            logger.LogInformation("Refresh token created for user: {UserId}", userId);
            return Result<bool>.Success(saved);
        }
        catch (Exception ex) {
            logger.LogError("Error creating refresh token user: {UserId}", userId);
            return Result<bool>.Failure("An Error occurred while creating refresh token", "TOKEN_ERROR");
        }
    }

    public async Task<Result<bool>> SaveRefreshTokenAsync(RefreshToken refreshTokens, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var entity = mapper.Map<RefreshTokenEntity>(refreshTokens);

            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);

            if(!saved) {
                logger.LogError("Failed to save refresh token for user: {UserId}", refreshTokens.UserId);
                return Result<bool>.Failure("Refresh token saved refresh token", "TOKEN_002")
;           }

            logger.LogInformation("Refresh token saved for user: {UserId}", refreshTokens.UserId);
            return Result<bool>.Success(saved);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error saving refresh token for user: {UserId}", refreshTokens.UserId);
            return Result<bool>.Failure("An error occurred while saving refresh token", "TOKEN_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry,
        CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var bucket = new RelationPredicateBucket();

            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);

            bucket.PredicateExpression.Add(filter);

            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket), cancellationToken);
            
            if(entity != null) {
                entity.RefreshToken = refreshToken;
                entity.ExpiresAt = expiry;
                var updated = await adapter.SaveEntityAsync(entity, cancellationToken);

                if(!updated) {
                    logger.LogError($"Failed to update refresh token for user: {userId}");
                    return Result<bool>.Failure("Failed to update refresh token", "TOKEN_003");
                }
                logger.LogInformation($"Refresh token updated for user: {userId}");
                return Result<bool>.Success(updated);
            }
            else {
                return await CreateRefreshTokenAsync(userId, refreshToken, expiry, cancellationToken);
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating refresh token for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while updating refresh token", "TOKEN_ERROR");
        }
    }

    public async Task<Result<string?>> GetRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var bucket = new RelationPredicateBucket();

            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);

            bucket.PredicateExpression.Add(filter);

            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket));

            if (entity == null) {
                logger.LogWarning($"No valid refresh token found for user: {userId}");
                return Result<string?>.Success(null);
            }
            return Result<string?>.Success(entity.RefreshToken);
        }
        catch (Exception ex) {
            logger.LogWarning(ex, $"Error getting refresh token for user: {userId}");
            return Result<string?>.Failure("An error occurred while retrieving refresh token");
        }
    }

    public async Task<Result<bool>> ValidateRefreshTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var bucket = new RelationPredicateBucket();

            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);

            bucket.PredicateExpression.Add(filter);

            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket));

            if (entity == null) {
                logger.LogWarning("Invalid or expired refresh token for user: {UserId}", userId);
                return Result<bool>.Success(false);
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error validating refresh token for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while validating refresh token", "TOKEN_ERROR");
        }
    }

    public async Task<Result<bool>> RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var bucket = new RelationPredicateBucket();

            var filter = new PredicateExpression();
            filter.Add(RefreshTokenFields.UserId == userId);
            filter.Add(RefreshTokenFields.Revoked == false);
            filter.Add(RefreshTokenFields.ExpiresAt > DateTime.UtcNow);

            bucket.PredicateExpression.Add(filter);

            var entity = await Task.Run(() => adapter.FetchNewEntity<RefreshTokenEntity>(bucket));

            if (entity == null) {
                logger.LogWarning("Failed to fetch refresh token by userId: {UserId}", userId);
                return Result<bool>.Success(false);
            }

            entity.Revoked = true;
            var saved = await adapter.SaveEntityAsync(entity);

            if (!saved) {
                logger.LogWarning("Invalid or expired refresh token for user: {UserId}", userId);
                return Result<bool>.Success(false);
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error validating revoke token for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while validating revoke token", "TOKEN_ERROR");
        }
    }

    public async Task<Result<bool>> UpdateLastLoginAsync(Guid userId, DateTime lastLogin, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var entity = new UserEntity(userId);
            var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);

            if (!fetched) {
                logger.LogWarning($"failed to fetch user by id : {userId}");
                return Result<bool>.Success(false);
            }
            entity.LastLoginAt = lastLogin;
            var saved =  await adapter.SaveEntityAsync(entity, cancellationToken);

            if (!saved) {
                logger.LogWarning($"failed to update last login for user id: {userId}");
                return Result<bool>.Success(false);
            }
            logger.LogInformation($"Update last login for user id: {userId}");
            return Result<bool>.Success(true);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error validating update last login for user: {UserId}", userId);
            return Result<bool>.Failure("An error occurred while validating update last login", "UPDATE_LAST_LOGIN");
        }
    }

    public async Task<Result<bool>> RecordFailedLoginAttemptAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserFields.Email == emailOrUsername);
            filter.AddWithOr(UserFields.Username == emailOrUsername);
            bucket.PredicateExpression.Add(filter);

            var userEntity = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

            if (userEntity == null) {
                logger.LogWarning("User not found for failed login attempt: {EmailOrUsername}", emailOrUsername);
                return Result<bool>.Success(false);
            }

            userEntity.FailedLoginAttempts = userEntity.FailedLoginAttempts + 1;
            userEntity.LastLoginAt = DateTime.UtcNow;

            var saved = await adapter.SaveEntityAsync(userEntity, cancellationToken);

            if (!saved) {
                logger.LogError("Failed to record failed login attempt for: {EmailOrUsername}", emailOrUsername);
                return Result<bool>.Failure("Failed to record login attempt", "LOGIN_001");
            }

            logger.LogInformation("Recorded failed login attempt for: {EmailOrUsername}", emailOrUsername);
            return Result<bool>.Success(true);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error recording failed login attempt for: {EmailOrUsername}", emailOrUsername);
            return Result<bool>.Failure("An error occurred while recording failed login attempt", "LOGIN_ERROR");
        }
    }

    public async Task<Result<int>> GetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        try
        {
            using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
            
            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserFields.Email == emailOrUsername);
            filter.AddWithOr(UserFields.Username == emailOrUsername);
            bucket.PredicateExpression.Add(filter);

            var userEntity = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

            if (userEntity == null)
            {
                logger.LogWarning("User not found for getting failed login attempts: {EmailOrUsername}", emailOrUsername);
                return Result<int>.Success(0);
            }

            var attempts = userEntity.FailedLoginAttempts;
            logger.LogInformation("Failed login attempts for {EmailOrUsername}: {Attempts}", emailOrUsername, attempts);
            return Result<int>.Success(attempts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting failed login attempts for: {EmailOrUsername}", emailOrUsername);
            return Result<int>.Failure("An error occurred while getting failed login attempts", "LOGIN_ERROR");
        }
    }

    public async Task<Result<bool>> ResetFailedLoginAttemptsAsync(string emailOrUsername, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

            var bucket = new RelationPredicateBucket();
            var filter = new PredicateExpression();
            filter.Add(UserFields.Email == emailOrUsername);
            filter.AddWithOr(UserFields.Username == emailOrUsername);
            bucket.PredicateExpression.Add(filter);

            var userEntity = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

            if (userEntity == null) {
                logger.LogWarning("User not found for resetting failed login attempts: {EmailOrUsername}", emailOrUsername);
                return Result<bool>.Success(false);
            }

            userEntity.FailedLoginAttempts = 0;
            userEntity.LastFailedLogin = null;

            var saved = await adapter.SaveEntityAsync(userEntity, cancellationToken);

            if (!saved) {
                logger.LogError("Failed to reset failed login attempts for: {EmailOrUsername}", emailOrUsername);
                return Result<bool>.Failure("Failed to reset login attempts", "LOGIN_002");
            }

            logger.LogInformation("Reset failed login attempts for: {EmailOrUsername}", emailOrUsername);
            return Result<bool>.Success(true);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error resetting failed login attempts for: {EmailOrUsername}", emailOrUsername);
            return Result<bool>.Failure("An error occurred while resetting failed login attempts", "LOGIN_ERROR");
        }
    }

    public async Task<Result<List<string>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try {
            using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
            var userRoleAssignment = new EntityCollection<UserRoleAssignmentEntity>();

            var prefetchPath = new PrefetchPath2(EntityType.UserRoleAssignmentEntity);
            prefetchPath.Add(UserRoleAssignmentEntity.PrefetchPathUserRole);

            var filter = new PredicateExpression(UserRoleAssignmentFields.UserId == userId);

            var queryParameter = new QueryParameters {
                CollectionToFetch = userRoleAssignment,
                FilterToUse = filter,
                PrefetchPathToUse = prefetchPath
            };

            await adapter.FetchEntityCollectionAsync(queryParameter, cancellationToken);

            var roleNames = userRoleAssignment
                .Select(ura => ura.UserRole.RoleName)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            logger.LogInformation("Retrieved {Count} roles for user: {UserId}", roleNames.Count, userId);
            return Result<List<string>>.Success(roleNames);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error getting user roles for: {UserId}", userId);
            return Result<List<string>>.Failure("An error occurred while getting user roles", "ROLE_ERROR");
        }
    }
}