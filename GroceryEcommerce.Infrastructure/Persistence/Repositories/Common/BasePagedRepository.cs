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
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger logger)
    : IPagedRepository<TDomainEntity>
    where TEntity : class, IEntityCore
    where TDomainEntity : class
{
    protected readonly DataAccessAdapter Adapter = adapter;
    protected readonly IMapper Mapper = mapper;
    protected readonly ICacheService CacheService = cacheService;
    protected readonly ILogger Logger = logger;

    public abstract IReadOnlyList<SearchableField> GetSearchableFields();
    public abstract string? GetDefaultSortField();
    public abstract IReadOnlyList<FieldMapping> GetFieldMappings();

    protected Task<Result<PagedResult<TDomainEntity>>> GetPagedConfiguredAsync(
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
        return GetPagedAsync(request, cancellationToken);
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
            var totalCount = await Adapter.FetchScalarAsync<int>(
                query.Select(() => Functions.CountRow()),
                cancellationToken
            );

            // Apply paging
            query = query.Page(request.Page, request.PageSize);

            // Fetch data
            var entities = await FetchEntitiesAsync(query, cancellationToken);
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

    // Abstract methods - Infrastructure layer sẽ implement
    protected abstract EntityQuery<TEntity> ApplySearch(EntityQuery<TEntity> query, string searchTerm);
    protected abstract EntityQuery<TEntity> ApplyFilter(EntityQuery<TEntity> query, FilterCriteria filter);
    protected abstract EntityQuery<TEntity> ApplySorting(EntityQuery<TEntity> query, string? sortBy, SortDirection sortDirection);
    protected abstract EntityQuery<TEntity> ApplyDefaultSorting(EntityQuery<TEntity> query);
    protected abstract Task<IList<TEntity>> FetchEntitiesAsync(EntityQuery<TEntity> query, CancellationToken cancellationToken);
}
