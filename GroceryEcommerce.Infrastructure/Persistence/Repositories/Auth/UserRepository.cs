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
        DataAccessAdapter scopedAdapter,
        IUnitOfWorkService unitOfWorkService,
        IMapper mapper,
        ICacheService cacheService,
        ILogger<UserRepository> logger
    ) : BasePagedRepository<UserEntity,User>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger),IUserRepository
    {

        public EntityField2? GetSortField(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "email" =>  UserFields.Email,
                "username" =>  UserFields.Username,
                "firstname" =>  UserFields.FirstName,
                "lastname" =>  UserFields.LastName,
                "phonenumber" =>  UserFields.PhoneNumber,
                "status" =>  UserFields.Status,
                "emailverified" =>  UserFields.EmailVerified,
                "phonenumberverified" =>  UserFields.PhoneVerified,
                "createdat" =>  UserFields.CreatedAt,
                "lastloginat" =>  UserFields.LastLoginAt,
                _ =>  UserFields.Username,
            };
        }
        public override IReadOnlyList<SearchableField> GetSearchableFields() {
            return new List<SearchableField>
            {
                new SearchableField("Email", typeof(string)),
                new SearchableField("Username", typeof(string)),
                new SearchableField("FirstName", typeof(string)),
                new SearchableField("LastName", typeof(string)),
                new SearchableField("PhoneNumber", typeof(string)),
                new SearchableField("Status", typeof(short)),
                new SearchableField("EmailVerified", typeof(bool)),
                new SearchableField("PhoneVerified", typeof(bool)),
                new SearchableField("CreatedAt", typeof(DateTime)),
                new SearchableField("LastLoginAt", typeof(DateTime?))
            }.AsReadOnly();
        }

        public override string GetDefaultSortField()
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

        protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
        {
            return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
            {
                ["email"] = UserFields.Email,
                ["username"] = UserFields.Username,
                ["firstname"] = UserFields.FirstName,
                ["lastname"] = UserFields.LastName,
                ["phonenumber"] = UserFields.PhoneNumber,
                ["status"] = UserFields.Status,
                ["emailverified"] = UserFields.EmailVerified,
                ["phonenumberverified"] = UserFields.PhoneVerified,
                ["createdat"] = UserFields.CreatedAt,
                ["lastloginat"] = UserFields.LastLoginAt,
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

        protected override async Task<IList<UserEntity>> FetchEntitiesAsync(EntityQuery<UserEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
        {
            var entitties = new EntityCollection<UserEntity>();
            await adapter.FetchQueryAsync(query, entitties, cancellationToken);
            return entitties;
        }
        
        public async Task<Result<bool>> AddAsync(User user, CancellationToken cancellationToken = default) {
            try {

                var entity = Mapper.Map<UserEntity>(user);
                entity.CreatedAt = DateTime.UtcNow;

                var saved = await GetAdapter().SaveEntityAsync(entity, cancellationToken);

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
                var deleted = await GetAdapter().DeleteEntityAsync(entity, cancellationToken);

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
            if (string.IsNullOrWhiteSpace(email)) {
                logger.LogWarning("Email is required");
                return Result<bool>.Failure("Invalid email.");
            }
            return await ExistsByCountAsync(UserFields.Email, email, cancellationToken);
        }
        
        public async Task<Result<bool>> ExistsByUsernameAsync(string username, CancellationToken cancellationToken = default) {
            if (string.IsNullOrWhiteSpace(username)) {
                logger.LogWarning("Username is required");
                return Result<bool>.Failure("Invalid username.");
            }
            return await ExistsByCountAsync(UserFields.Username, username, cancellationToken);
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
                await GetAdapter().FetchQueryAsync(query, entities, cancellationToken);

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

        public Task<Result<User?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<User?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {
            try {

                var entity = await GetAdapter().FetchFirstAsync(
                    new QueryFactory().User.Where(UserFields.Email == email),
                    cancellationToken
                );
                if (entity is null) {
                    logger.LogWarning("User not found by email: {Email}", email);
                    return Result<User?>.Success(null);
                }
                var user = Mapper.Map<User>(entity);
                logger.LogInformation("User fetched successfully: {Email}", email);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by email: {Email}", email);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<User?>> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) {
            if (string.IsNullOrWhiteSpace(username)) {
                logger.LogWarning("Username is required");
                return Result<User?>.Failure("Invalid username.");
            }
            return await GetSingleAsync(UserFields.Username, username, "User_ByUsername", TimeSpan.FromHours(1), cancellationToken);
        }

        public async Task<Result<User?>> GetUserByEmailOrUsernameAsync(string emailOrUsername, CancellationToken cancellationToken = default) {
            if (string.IsNullOrWhiteSpace(emailOrUsername)) {
                logger.LogWarning("Email or username is required");
                return Result<User?>.Failure("Invalid email or username.");
            }
    
            try {
                var query = new QueryFactory().User
                    .Where(UserFields.Email == emailOrUsername | UserFields.Username == emailOrUsername);
        
                var entity = await GetAdapter().FetchFirstAsync(query, cancellationToken);
        
                if (entity is null) {
                    logger.LogWarning("User not found by email or username: {EmailOrUsername}", emailOrUsername);
                    return Result<User?>.Success(null);
                }
        
                var user = Mapper.Map<User>(entity);
                logger.LogInformation("User fetched successfully: {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Success(user);
            }
            catch (Exception ex) {
                logger.LogError(ex, "Error getting user by email or username: {EmailOrUsername}", emailOrUsername);
                return Result<User?>.Failure("An error occurred while retrieving user", "USER_GET_ERROR");
            }
        }

        public async Task<Result<bool>> UpdateAsync(User user, CancellationToken cancellationToken = default) {
            try {
                var query = new QueryFactory().User.Where(UserFields.UserId == user.UserId);

                var entity = await GetAdapter().FetchFirstAsync(query, cancellationToken);
                if (entity is null) {
                    logger.LogWarning("User not found for update: {UserId}", user.UserId);
                    return Result<bool>.Failure("User not found", "USER_UPDATE_001");
                }

                entity.UpdatedAt = DateTime.UtcNow;
                entity = Mapper.Map(user, entity);
                var saved = await GetAdapter().SaveEntityAsync(entity, cancellationToken);
                if (!saved) {
                    logger.LogError("Failed to update user: {UserId}", user.UserId);
                    return Result<bool>.Failure("Failed to update user", "USER_UPDATE_001");
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
