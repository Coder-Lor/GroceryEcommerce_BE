using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.QuerySpec;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class BrandRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<BrandRepository> logger
    ) : BasePagedRepository<BrandEntity, Brand>(adapter, mapper, cacheService, logger), IBrandRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        throw new NotImplementedException();
    }

    public override string? GetDefaultSortField()
    {
        throw new NotImplementedException();
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<BrandEntity> ApplySearch(EntityQuery<BrandEntity> query, string searchTerm)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<BrandEntity> ApplyFilter(EntityQuery<BrandEntity> query, FilterCriteria filter)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<BrandEntity> ApplySorting(EntityQuery<BrandEntity> query, string? sortBy, SortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<BrandEntity> ApplyDefaultSorting(EntityQuery<BrandEntity> query)
    {
        throw new NotImplementedException();
    }

    protected override Task<IList<BrandEntity>> FetchEntitiesAsync(EntityQuery<BrandEntity> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Brand?>> GetByIdAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Brand?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Brand?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Brand>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Brand>> CreateAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(Brand brand, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Brand>>> GetActiveBrandsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Brand>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> IsBrandInUseAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetProductCountByBrandAsync(Guid brandId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}