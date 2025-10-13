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

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Auth
{
    public class UserRepository(
        DataAccessAdapter adapter,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<UserRepository> logger
    ) : BasePagedRepository<UserEntity,User>(adapter, mapper, cacheService, logger),IUserRepository
    {

        public EntityField2? GetSortField(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "email" => (EntityField2) UserFields.Email,
                "username" => (EntityField2) UserFields.Username,
                "firstname" => (EntityField2) UserFields.FirstName,
                "lastname" => (EntityField2) UserFields.LastName,
                "phonenumber" => (EntityField2) UserFields.PhoneNumber,
                "status" => (EntityField2) UserFields.Status,
                "emailverified" => (EntityField2) UserFields.EmailVerified,
                "phonenumberverified" => (EntityField2) UserFields.PhoneVerified,
                "createdat" => (EntityField2) UserFields.CreatedAt,
                "lastloginat" => (EntityField2) UserFields.LastLoginAt,
                _ => (EntityField2) UserFields.Username,
            };
        }
        public override IReadOnlyList<SearchableField> GetSearchableFields() {
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

        public override string? GetDefaultSortField()
        {
            return "Username";
        }

        public override IReadOnlyList<FieldMapping> GetFieldMappings()
        {
            return new List<FieldMapping>
            {
                new FieldMapping
                {
                    FieldName = "Email", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "Username", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "FirstName", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "LastName", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "PhoneNumber", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "EmailVerified", FieldType = typeof(bool), IsSearchable = false, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "PhoneVerified", FieldType = typeof(bool), IsSearchable = false, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true,
                    IsFilterable = true
                },
                new FieldMapping
                {
                    FieldName = "LastLoginAt", FieldType = typeof(DateTime?), IsSearchable = false, IsSortable = true,
                    IsFilterable = true
                }
            };
        }

        protected override EntityQuery<UserEntity> ApplySearch(EntityQuery<UserEntity> query, string searchTerm)
        {
            return query.Where(
                UserFields.Username.Contains(searchTerm) |
                UserFields.Email.Contains(searchTerm) |
                UserFields.FirstName.Contains(searchTerm) |
                UserFields.LastName.Contains(searchTerm)
            );
        }

        protected override EntityQuery<UserEntity> ApplyFilter(EntityQuery<UserEntity> query, FilterCriteria filter)
        {
            return filter.Operator switch
            {
                FilterOperator.Equals when filter.FieldName.Equals("email", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.Email == filter.Value.ToString()),

                FilterOperator.Contains when filter.FieldName.Equals("email", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.Email.Contains(filter.Value.ToString())),

                FilterOperator.Equals when filter.FieldName.Equals("username", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.Username == filter.Value.ToString()),

                FilterOperator.Contains when filter.FieldName.Equals("username", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.Username.Contains(filter.Value.ToString())),

                FilterOperator.Equals when filter.FieldName.Equals("firstname", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.FirstName == filter.Value.ToString()),
                FilterOperator.Contains when filter.FieldName.Equals("firstname", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.FirstName.Contains(filter.Value.ToString())),

                FilterOperator.Equals when filter.FieldName.Equals("lastname", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.LastName == filter.Value.ToString()),
                FilterOperator.Contains when filter.FieldName.Equals("lastname", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.LastName.Contains(filter.Value.ToString())),

                FilterOperator.Equals when filter.FieldName.Equals("phonenumber", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.PhoneNumber == filter.Value.ToString()),
                FilterOperator.Contains when filter.FieldName.Equals("phonenumber", StringComparison.OrdinalIgnoreCase)
                    =>
                    query.Where(UserFields.PhoneNumber.Contains(filter.Value.ToString())),

                FilterOperator.Equals when filter.FieldName.Equals("status", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.Status == Convert.ToInt16(filter.Value)),
                
                FilterOperator.GreaterThan when filter.FieldName.Equals("createdat", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.CreatedAt > Convert.ToDateTime(filter.Value)),
                
                FilterOperator.LessThan when filter.FieldName.Equals("createdat", StringComparison.OrdinalIgnoreCase) =>
                    query.Where(UserFields.CreatedAt < Convert.ToDateTime(filter.Value)),
                
                _ => query
            };
        }

        protected override EntityQuery<UserEntity> ApplySorting(EntityQuery<UserEntity> query, string? sortBy, SortDirection sortDirection)
        {
            var sortField = GetSortField(sortBy);
            if (sortField is null) return query;

            return sortDirection == SortDirection.Descending
                ? query.OrderBy(sortField.Descending())
                : query.OrderBy(sortField.Ascending());
        }

        protected override EntityQuery<UserEntity> ApplyDefaultSorting(EntityQuery<UserEntity> query)
        {
            return query.OrderBy(UserFields.Username.Ascending());       
        }

        protected override async Task<IList<UserEntity>> FetchEntitiesAsync(EntityQuery<UserEntity> query, CancellationToken cancellationToken)
        {
            var entitties = new EntityCollection<UserEntity>();
            await adapter.FetchQueryAsync(query, entitties, cancellationToken);
            return entitties;
        }
        
        public async Task<Result<bool>> AddAsync(User user, CancellationToken cancellationToken = default) {
            try {

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
            try
            {
                var qf = new QueryFactory();
                var q = qf.User
                    .Where(UserFields.Email == email)
                    .Limit(1);

                var entities = new EntityCollection<UserEntity>();
                await adapter.FetchQueryAsync(q, entities, cancellationToken);

                return Result<bool>.Success(entities.Count > 0);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error checking if email exists: {Email}", email);
                return Result<bool>.Failure("An error occurred while checking email", "USER_EXISTS_ERROR");
            }
        }
        
        public async Task<Result<bool>> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default) {
            try
            {
                var qf = new QueryFactory();
                var q = qf.User
                    .Where(UserFields.Username == username)
                    .Limit(1);

                var entities = new EntityCollection<UserEntity>();
                await adapter.FetchQueryAsync(q, entities, cancellationToken);

                return Result<bool>.Success(entities.Count > 0);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking if username exists: {Username}", username);
                return Result<bool>.Failure("An error occurred while checking username", "USER_EXISTS_ERROR");
            }
        }

        public async Task<Result<(bool emailExists, bool usernameExists)>> CheckUserExistenceAsync(string email, string username,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var qf = new QueryFactory();
                var query = qf.User
                    .Where(UserFields.Email == email | UserFields.Username == username)
                    .Limit(2);
                var entities = new EntityCollection<UserEntity>();
                await adapter.FetchQueryAsync(query, entities, cancellationToken);

                var emailExist = entities.Any(e => e.Email == email);
                var usernameExist = entities.Any(e => e.Username == username);
                return Result<(bool, bool)>.Success((emailExist, usernameExist));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error checking user existence by email or username: {Email}, {Username}", email,
                    username);
                return Result<(bool, bool)>.Failure("An error occurred while checking user existence", "USER_CHECK_ERROR");
            }
        }

        public async Task<Result<User?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
            try {

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

        public async Task<Result<User?>> GetUserByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default) {
            try {

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
