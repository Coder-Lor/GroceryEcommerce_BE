using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class BrandRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<BrandRepository> logger
    ) : BasePagedRepository<BrandEntity, Brand>(adapter, mapper, cacheService, logger), IBrandRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => BrandFields.Name,
            "slug" => BrandFields.Slug,
            "createdat" => BrandFields.CreatedAt,
            "status" => BrandFields.Status,
            _ => null
        };
    }
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("BrandId", typeof(Guid)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("CreatedBy", typeof(string)),
            new SearchableField("Description", typeof(string)),
            new SearchableField("LogoUrl", typeof(string)),
            new SearchableField("Name", typeof(string)),
            new SearchableField("Slug", typeof(string)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("UpdatedAt", typeof(DateTime)),
            new SearchableField("UpdatedBy", typeof(string)),
            new SearchableField("Website", typeof(string))
        };
    }
    public override string GetDefaultSortField()
    {
        return "Name";   
    }
    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping
            {
                FieldName = "BrandId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "CreatedBy", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Description", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "LogoUrl", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Slug", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Status", FieldType = typeof(short), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "UpdatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "UpdatedBy", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Website", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            }
        };
    }
    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "brandid", BrandFields.BrandId },
            { "createdat", BrandFields.CreatedAt },
            { "createdby", BrandFields.CreatedBy },
            { "description", BrandFields.Description },
            { "logourl", BrandFields.LogoUrl },
            { "name", BrandFields.Name },
            { "slug", BrandFields.Slug },
            { "status", BrandFields.Status },
            { "updatedat", BrandFields.UpdatedAt },
            { "updatedby", BrandFields.UpdatedBy },
            { "website", BrandFields.Website }
        };
    }
    protected override EntityQuery<BrandEntity> ApplySearch(EntityQuery<BrandEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        return query.Where(
            BrandFields.Name.Contains(searchTerm) |
            BrandFields.Slug.Contains(searchTerm) 
        );
    }
    
    protected override EntityQuery<BrandEntity> ApplySorting(EntityQuery<BrandEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? BrandFields.Name;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<BrandEntity> ApplyDefaultSorting(EntityQuery<BrandEntity> query)
    {
        return query.OrderBy(BrandFields.Name.Ascending());   
    }

    protected override async Task<IList<BrandEntity>> FetchEntitiesAsync(EntityQuery<BrandEntity> query, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<BrandEntity>();
        await Adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public Task<Result<Brand?>> GetByIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        if (brandId == Guid.Empty)
        {
            logger.LogWarning("Brand id is required");
            return Task.FromResult(Result<Brand?>.Failure("Invalid brand ID."));
        }
        return GetSingleAsync(BrandFields.BrandId, brandId, "Brand", TimeSpan.FromHours(1), cancellationToken);   
    }

    public Task<Result<Brand?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogWarning("Brand name is required");
            return Task.FromResult(Result<Brand?>.Failure("Brand name is required."));
        }
        return GetSingleAsync(BrandFields.Name, name.Trim(), "Brand", TimeSpan.FromHours(1), cancellationToken);
    }

    public Task<Result<Brand?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Brand slug is required");
            return Task.FromResult(Result<Brand?>.Failure("Brand slug is required."));
        }
        return GetSingleAsync(BrandFields.Slug, slug.Trim(), "Brand", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<List<Brand>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            const string cacheKey = "Brands_All";
            
            var cached = await CacheService.GetAsync<List<Brand>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Retrieved all brands from cache.");
                return Result<List<Brand>>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.Brand
                .OrderBy(BrandFields.Name.Ascending());
            
            var entities = new EntityCollection<BrandEntity>();
            
            await Adapter.FetchQueryAsync(query,entities, cancellationToken);
            var brandList = Mapper.Map<List<Brand>>(entities);

            // Cache 15 phút
            await CacheService.SetAsync(cacheKey, brandList, TimeSpan.FromMinutes(15), cancellationToken);

            return Result<List<Brand>>.Success(brandList);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all brands");
            return Result<List<Brand>>.Failure("An error occurred while retrieving all brands.");
        }
    }

    public async Task<Result<Brand>> CreateAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<BrandEntity>(brand);
            entity.IsNew = true;

            var saved = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                var cacheKeyPrefix = "Brand";
                var cacheKey = $"{cacheKeyPrefix}_{entity.BrandId}";
                await CacheService.RemoveAsync(cacheKey, cancellationToken);
                logger.LogInformation("Brand created: {Name}", brand.Name);
                return Result<Brand>.Success(Mapper.Map<Brand>(entity));
            }
            logger.LogWarning("Failed to create brand: {Name}", brand.Name);
            return Result<Brand>.Failure("Failed to create brand.");
        }
        catch (Exception ex)
        {
            logger.LogError("Error creating brand: {Message}", ex.Message);
            return Result<Brand>.Failure("An error occurred while creating the brand.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<BrandEntity>(brand);
            entity.IsNew = false;

            var saved = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                var cacheKeyPrefix = "Brand";
                var cacheKey = $"{cacheKeyPrefix}_{entity.BrandId}";
                await CacheService.RemoveAsync(cacheKey, cancellationToken);
                logger.LogInformation("Brand updated: {Name}", brand.Name);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Failed to update brand: {Name}", brand.Name);
            return Result<bool>.Failure("Failed to update brand.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating brand: {Name}", brand.Name);
            return Result<bool>.Failure("An error occurred while updating the brand.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (brandId == Guid.Empty)
            {
                logger.LogWarning("Brand id is required");
                return Result<bool>.Failure("Invalid brand ID.");
            }
            var entity = new BrandEntity(brandId);
            var deleted = await Adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted)
            {
                var cacheKeyPrefix = "Brand";
                var cacheKey = $"{cacheKeyPrefix}_{brandId}";
                await CacheService.RemoveAsync(cacheKey, cancellationToken);
                logger.LogInformation("Brand deleted: {BrandId}", brandId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Failed to delete brand: {BrandId}", brandId);
            return Result<bool>.Failure("Failed to delete brand.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting brand: {BrandId}", brandId);
            return Result<bool>.Failure("An error occurred while deleting the brand.");
        }
    }

    public Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogWarning("Brand name is required");
            return Task.FromResult(Result<bool>.Failure("Brand name is required."));
        }
        return ExistsByCountAsync(BrandFields.Name, name.Trim(), cancellationToken);
    }

    public Task<Result<bool>> ExistsAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        if (brandId == Guid.Empty)
        {
            logger.LogWarning("Brand id is required");
            return Task.FromResult(Result<bool>.Failure("Invalid brand ID."));
        }
        return ExistsByCountAsync(BrandFields.BrandId, brandId, cancellationToken);
    }

    public async Task<Result<PagedResult<Brand>>> GetActiveBrandsAsync(PagedRequest request, CancellationToken cancellationToken = default) 
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Status", 1), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Brand>>> SearchByNameAsync(PagedRequest request, string searchTerm, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Name", searchTerm), cancellationToken: cancellationToken);

    public async Task<Result<bool>> IsBrandInUseAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        if (brandId == Guid.Empty)
        {
            logger.LogWarning("Brand id is required");
            return Result<bool>.Failure("Invalid brand ID.");
        }
        return await ExistsByCountAsync(ProductFields.BrandId, brandId, cancellationToken);   
    }

    public async Task<Result<int>> GetProductCountByBrandAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        if (brandId == Guid.Empty)
        {
            logger.LogWarning("Brand id is required");
            return Result<int>.Failure("Invalid brand ID.");
        }
        return await CountByFieldAsync(BrandFields.BrandId, brandId, cancellationToken);
    }
}