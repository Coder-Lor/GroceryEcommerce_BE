using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.FactoryClasses;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;

public abstract class BasePagedRepository<TEntity, TDomainEntity>(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger logger)
    : IPagedRepository<TDomainEntity>
    where TEntity : EntityBase2
    where TDomainEntity : class
{
    protected readonly DataAccessAdapter ScopedAdapter = scopedAdapter;
    protected readonly IUnitOfWorkService UnitOfWorkService = unitOfWorkService;
    protected readonly IMapper Mapper = mapper;
    protected readonly ICacheService CacheService = cacheService;
    protected readonly ILogger Logger = logger;

    // Method để lấy adapter phù hợp
    protected DataAccessAdapter GetAdapter()
    {
        // Nếu có active transaction, dùng adapter của transaction
        if (UnitOfWorkService.HasActiveTransaction)
        {
            return UnitOfWorkService.GetAdapter();
        }
        
        // Nếu không, dùng scoped adapter
        return ScopedAdapter;
    }


    public abstract IReadOnlyList<SearchableField> GetSearchableFields();
    public abstract string? GetDefaultSortField();
    public abstract IReadOnlyList<FieldMapping> GetFieldMappings();

    protected async Task<Result<PagedResult<TDomainEntity>>> GetPagedConfiguredAsync(
        PagedRequest request,
        Action<PagedRequest> configure,
        string? defaultSortField = null,
        SortDirection defaultSortDirection = SortDirection.Ascending,
        CancellationToken cancellationToken = default)
    {
        configure(request);
        if (!request.HasSorting && !string.IsNullOrWhiteSpace(defaultSortField))
        {
            request.WithSorting(defaultSortField, defaultSortDirection);
        }
        return await GetPagedAsync(request, cancellationToken);
    }

    protected async Task<Result<TDomainEntity?>> GetSingleAsync<TValue>(
        EntityField2 field,
        TValue value,
        string cacheKeyPrefix,
        TimeSpan cacheTtl,
        CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = $"{cacheKeyPrefix}_{value}";
            var cached = await CacheService.GetAsync<TDomainEntity>(cacheKey, cancellationToken);
            if (cached != null)
            {
                Logger.LogInformation("{EntityName} fetched from cache by {Field}: {Value}", 
                    typeof(TDomainEntity).Name, field.Name, value);
                return Result<TDomainEntity?>.Success(cached);
            }

            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var qf = new QueryFactory();
            var query = qf.Create<TEntity>().Where(field == value);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                Logger.LogWarning("{EntityName} not found by {Field}: {Value}", 
                    typeof(TDomainEntity).Name, field.Name, value);
                return Result<TDomainEntity?>.Failure($"{typeof(TDomainEntity).Name} not found.");
            }

            var domainEntity = Mapper.Map<TDomainEntity>(entity);
            await CacheService.SetAsync(cacheKey, domainEntity, cacheTtl, cancellationToken);
            Logger.LogInformation("{EntityName} fetched from database and cached by {Field}: {Value}", 
                typeof(TDomainEntity).Name, field.Name, value);
            return Result<TDomainEntity?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting {EntityName} by {Field}: {Value}", 
                typeof(TDomainEntity).Name, field.Name, value);
            return Result<TDomainEntity?>.Failure($"An error occurred while fetching {typeof(TDomainEntity).Name}.");
        }
    }

    protected async Task<Result<bool>> ExistsByCountAsync<TValue>(
        EntityField2 field,
        TValue value,
        CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var qf = new QueryFactory();
            var countQuery = qf.Create<TEntity>()
                .Where(field == value)
                .Limit(1)
                .Select(() => Functions.CountRow());
            var count = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);
            Logger.LogInformation("Exists check for {EntityName} by {Field}: {Value} -> {Exists}", 
                typeof(TDomainEntity).Name, field.Name, value, count > 0);
            return Result<bool>.Success(count > 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking if {EntityName} exists by {Field}: {Value}", 
                typeof(TDomainEntity).Name, field.Name, value);
            return Result<bool>.Failure($"An error occurred while checking if {typeof(TDomainEntity).Name} exists.");
        }
    }

    protected async Task<Result<int>> CountByFieldAsync<TValue>(
        EntityField2 field,
        TValue value,
        CancellationToken cancellationToken)
    {
        try
        {
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var qf = new QueryFactory();
            var query = qf.Create<TEntity>()
                .Where(field == value)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            Logger.LogInformation("Count for {EntityName} by {Field}: {Value} -> {Count}", 
                typeof(TDomainEntity).Name, field.Name, value, count);
            
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting {EntityName} by {Field}: {Value}", 
                typeof(TDomainEntity).Name, field.Name, value);
            return Result<int>.Failure($"An error occurred while counting {typeof(TDomainEntity).Name}.");
        }
    }

    public virtual async Task<Result<PagedResult<TDomainEntity>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            request.AvailableFields = GetSearchableFields();
            var validation = request.Validate();
            if (validation != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                Logger.LogWarning("Invalid paged request: {Errors}", validation?.ErrorMessage);
                return Result<PagedResult<TDomainEntity>>.Failure(validation?.ErrorMessage ?? "Invalid paged request");
            }

            // Generate cache key
            var cacheKey = request.GenerateCacheKey(typeof(TDomainEntity).Name);
            var cached = await CacheService.GetAsync<PagedResult<TDomainEntity>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                Logger.LogInformation("Paged {EntityName} fetched from cache: Page {Page}, PageSize {PageSize}", 
                    typeof(TDomainEntity).Name, request.Page, request.PageSize);
                return Result<PagedResult<TDomainEntity>>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.Create<TEntity>();

            // Apply search
            if (request.HasSearch)
            {
                query = ApplySearch(query, request.Search ?? string.Empty);
            }

            // Apply filters
            if (request.HasFilters)
            {
                foreach (var filter in request.Filters)
                {
                    query = ApplyFilter(query, filter);
                }
            }

            // Apply sorting
            if (request.HasSorting)
            {
                query = ApplySorting(query, request.SortBy, request.SortDirection);
            }
            else
            {
                query = ApplyDefaultSorting(query);
            }

            // Get total count
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var totalCount = await adapter.FetchScalarAsync<int>(
                query.Select(() => Functions.CountRow()),
                cancellationToken
            );

            // Apply paging
            query = query.Page(request.Page, request.PageSize);

            // Fetch data
            var entities = await FetchEntitiesAsync(query, adapter, cancellationToken);
            var domainEntities = Mapper.Map<List<TDomainEntity>>(entities);

            var result = new PagedResult<TDomainEntity>(domainEntities, totalCount, request.Page, request.PageSize);

            // Cache result
            await CacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);

            Logger.LogInformation("Paged {EntityName} fetched: Page {Page}, PageSize {PageSize}, Total {Total}", 
                typeof(TDomainEntity).Name, request.Page, request.PageSize, totalCount);
            
            return Result<PagedResult<TDomainEntity>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching {EntityName} by page", typeof(TDomainEntity).Name);
            return Result<PagedResult<TDomainEntity>>.Failure($"An error occurred while fetching {typeof(TDomainEntity).Name}.");
        }
    }

    protected virtual EntityQuery<TEntity> ApplyFilter(EntityQuery<TEntity> query, FilterCriteria filter)
    {
        var fieldMap = GetFieldMap();
        if (!fieldMap.TryGetValue(filter.FieldName, out var field)) return query;
        
        return filter.Operator switch
        {
            FilterOperator.Equals => query.Where(field == filter.Value),
            FilterOperator.NotEquals => query.Where(field != filter.Value),
            FilterOperator.Contains => query.Where(field.Contains(filter.Value.ToString())),
            FilterOperator.NotContains => query.Where(!field.Contains(filter.Value.ToString())),
            FilterOperator.GreaterThan => query.Where(field > Convert.ToDecimal(filter.Value)),
            FilterOperator.LessThan => query.Where(field < Convert.ToDecimal(filter.Value)),
            FilterOperator.GreaterThanOrEqual => query.Where(field >= Convert.ToDecimal(filter.Value)),
            FilterOperator.LessThanOrEqual => query.Where(field <= Convert.ToDecimal(filter.Value)),
            FilterOperator.In => query.Where(field.In(filter.Value.ToString())),
            FilterOperator.NotIn => query.Where(field.NotIn(filter.Value.ToString())),
            FilterOperator.StartsWith => query.Where(field.StartsWith(filter.Value.ToString())),
            FilterOperator.EndsWith => query.Where(field.EndsWith(filter.Value.ToString())),
            FilterOperator.IsNull => query.Where(field.IsNull()),
            FilterOperator.IsNotNull => query.Where(field.IsNotNull()),
            _ => query
        };
    }
    // Abstract methods - Infrastructure layer sẽ implement
    protected abstract IReadOnlyDictionary<string, EntityField2> GetFieldMap();
    protected abstract EntityQuery<TEntity> ApplySearch(EntityQuery<TEntity> query, string searchTerm);
    protected abstract EntityQuery<TEntity> ApplySorting(EntityQuery<TEntity> query, string? sortBy, SortDirection sortDirection);
    protected abstract EntityQuery<TEntity> ApplyDefaultSorting(EntityQuery<TEntity> query);
    protected abstract Task<IList<TEntity>> FetchEntitiesAsync(EntityQuery<TEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken);
}
