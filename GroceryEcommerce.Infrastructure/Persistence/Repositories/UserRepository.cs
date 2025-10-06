using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories
{
    public class UserRepository(
        IDataAccessAdapterFactory adapterFactory,
        IMapper mapper,
        ILogger<UserRepository> logger
    ) :IUserRepository
    {
        public async Task<Result<bool>> AddAsync(User user, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var entity = mapper.Map<UserEntity>(user);
                entity.CreatedAt = DateTime.UtcNow;

                var saved = await adapter.SaveEntityAsync(entity, cancellationToken);

                if (!saved) {
                    logger.LogError("Failed to add user: {Email}", user.Email);
                    return Result<bool>.Failure("Failed to add user", "USER_ADD_001");
                }

                logger.LogInformation("User added successfully: {UserId}", entity.UserId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error adding user: {Email}", user.Email);
                return Result<bool>.Failure("An error occurred while adding user", "USER_ADD_ERROR");
            }
        }

        public async Task<Result<bool>> DeleteAsync(User user, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var entity = new UserEntity(user.UserId);
                var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);

                if (!deleted) {
                    logger.LogWarning("Failed to delete user: {UserId}", user.UserId);
                    return Result<bool>.Failure("User not found or already deleted", "USER_DELETE_001");
                }

                logger.LogInformation("User deleted successfully: {UserId}", user.UserId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error deleting user: {UserId}", user.UserId);
                return Result<bool>.Failure("An error occurred while deleting user", "USER_DELETE_ERROR");
            }
        }

        public async Task<Result<bool>> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
                var bucket = new RelationPredicateBucket(UserFields.Email == email);
                var collection = new EntityCollection<UserEntity>();
                var totalCount = await Task.Run(() => adapter.GetDbCount(collection, bucket), cancellationToken);

                return Result<bool>.Success(totalCount > 0);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error checking if email exists: {Email}", email);
                return Result<bool>.Failure("An error occurred while checking email", "USER_EXISTS_ERROR");
            }
        }
        
        public async Task<Result<bool>> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default) {
            try
            {
                using var adapter = (DataAccessAdapter)adapterFactory.CreateAdapter();
                var bucket = new RelationPredicateBucket(UserFields.Username == username);
                var collection = new EntityCollection<UserEntity>();
                
                var totalCount = await Task.Run(() => adapter.GetDbCount(collection, bucket), cancellationToken);
                return Result<bool>.Success(totalCount > 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if username exists: {Username}", username);
                return Result<bool>.Failure("An error occurred while checking username", "USER_EXISTS_ERROR");
            }
        }

        public async Task<Result<User?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var entity = new UserEntity();
                var filter = new PredicateExpression(UserFields.Email == email);
                var fetched = await Task.Run(() => adapter.FetchEntityUsingUniqueConstraint(entity, filter), cancellationToken);

                if (!fetched) {
                    logger.LogWarning("User not found by email: {Email}", email);
                    return Result<User?>.Success(null);
                }

                var user = mapper.Map<User>(entity);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by email: {Email}", email);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<User?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var entity = new UserEntity(id);
                var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);

                if (!fetched) {
                    logger.LogWarning("User not found by id: {UserId}", id);
                    return Result<User?>.Success(null);
                }

                var user = mapper.Map<User>(entity);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by id: {UserId}", id);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<User?>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var bucket = new RelationPredicateBucket(UserFields.Username == username);
                var entity = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

                if (entity == null) {
                    logger.LogWarning("User not found by username: {Username}", username);
                    return Result<User?>.Success(null);
                }

                var user = mapper.Map<User>(entity);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by username: {Username}", username);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<PagedResult<User>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();
                var qf = new QueryFactory();
                var q = qf.User;

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(request.Search)) {
                    var searchPattern = $"%{request.Search}%";
                    q.Where(
                        UserFields.Email.Like(searchPattern)
                        .Or(UserFields.FirstName.Like(searchPattern))
                        .Or(UserFields.LastName.Like(searchPattern))
                        .Or(UserFields.Username.Like(searchPattern))
                    );
                }

                // Apply custom filters
                if (request.Filters != null && request.Filters.Any()) {
                    foreach (var filter in request.Filters) {
                        switch (filter.Key.ToLower()) {
                            case "status":
                            if (filter.Value is short statusValue)
                                q.Where(UserFields.Status == statusValue);
                            break;
                            case "emailverified":
                            if (filter.Value is bool emailVerified)
                                q.Where(UserFields.EmailVerified == emailVerified);
                            break;
                            case "phoneverified":
                            if (filter.Value is bool phoneVerified)
                                q.Where(UserFields.PhoneVerified == phoneVerified);
                            break;
                        }
                    }
                }

                // Apply sorting
                if (!string.IsNullOrWhiteSpace(request.SortBy)) {
                    var sortField = request.SortBy.ToLower() switch {
                        "email" => UserFields.Email,
                        "firstname" => UserFields.FirstName,
                        "lastname" => UserFields.LastName,
                        "username" => UserFields.Username,
                        "createdat" => UserFields.CreatedAt,
                        "lastloginat" => UserFields.LastLoginAt,
                        _ => UserFields.CreatedAt
                    };

                    if (request.SortDescending)
                        q.OrderBy(sortField.Descending());
                    else
                        q.OrderBy(sortField.Ascending());
                }
                else {
                    q.OrderBy(UserFields.CreatedAt.Descending());
                }

                q.OrderBy(UserFields.CreatedAt.Descending());

                // Apply pagination
                q.Page(request.Page, request.PageSize);
                var totalCount = await adapter.FetchScalarAsync<int>(
                    q.Select(() => Functions.CountRow()),
                    cancellationToken
                );
                
                var entities = new EntityCollection<UserEntity>();
                await adapter.FetchQueryAsync(q, entities, cancellationToken);


                var users = mapper.Map<List<User>>(entities);
                var pagedResult = new PagedResult<User>(users, totalCount, request.Page, request.PageSize);

                logger.LogInformation("Retrieved {Count} users (Page {Page}/{TotalPages})",
                    users.Count, request.Page, pagedResult.TotalPages);

                return Result<PagedResult<User>>.Success(pagedResult);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting paged users");
                return Result<PagedResult<User>>.Failure("An error occurred while retrieving users", "USER_PAGED_ERROR");
            }
        }

        public IReadOnlyList<SearchableField> GetSearchableFields() {
            return new List<SearchableField>
            {
                new SearchableField("Email", typeof(string), true, true),
                new SearchableField("Username", typeof(string), true, true),
                new SearchableField("FirstName", typeof(string), true, true),
                new SearchableField("LastName", typeof(string), true, true),
                new SearchableField("PhoneNumber", typeof(string), true, false),
                new SearchableField("Status", typeof(short), false, true),
                new SearchableField("EmailVerified", typeof(bool), false, true),
                new SearchableField("PhoneVerified", typeof(bool), false, true),
                new SearchableField("CreatedAt", typeof(DateTime), false, true),
                new SearchableField("LastLoginAt", typeof(DateTime?), false, true)
            }.AsReadOnly();
        }

        public async Task<Result<User?>> GetUserByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var bucket = new RelationPredicateBucket(UserFields.Email == emailOrUsername);
                bucket.PredicateExpression.AddWithOr(UserFields.Username == emailOrUsername);

                var entity = await Task.Run(() => adapter.FetchNewEntity<UserEntity>(bucket), cancellationToken);

                if (entity == null) {
                    logger.LogWarning("User not found by email or username: {EmailOrUsername}", emailOrUsername);
                    return Result<User?>.Success(null);
                }

                var user = mapper.Map<User>(entity);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by email or username: {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken = default) {
            try {
                using var adapter = (DataAccessAdapter) adapterFactory.CreateAdapter();

                var entity = new UserEntity(user.UserId);
                var fetched = await Task.Run(() => adapter.FetchEntity(entity), cancellationToken);

                if (!fetched) {
                    logger.LogWarning("User not found for update: {UserId}", user.UserId);
                    return Result<bool>.Failure("User not found", "USER_UPDATE_001");
                }

                // Map updated fields
                mapper.Map(user, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                var saved = await adapter.SaveEntityAsync(entity, cancellationToken);

                if (!saved) {
                    logger.LogError("Failed to update user: {UserId}", user.UserId);
                    return Result<bool>.Failure("Failed to update user", "USER_UPDATE_002");
                }

                logger.LogInformation("User updated successfully: {UserId}", user.UserId);
                return Result<bool>.Success(true);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error updating user: {UserId}", user.UserId);
                return Result<bool>.Failure("An error occurred while updating user", "USER_UPDATE_ERROR");
            }
        }
    }
}
