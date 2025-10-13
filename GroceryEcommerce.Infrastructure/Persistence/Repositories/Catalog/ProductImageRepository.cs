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

public class ProductImageRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductImageRepository> logger
) : BasePagedRepository<ProductImageEntity, ProductImage>(adapter, mapper, cacheService, logger), IProductImageRepository
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

    protected override EntityQuery<ProductImageEntity> ApplySearch(EntityQuery<ProductImageEntity> query, string searchTerm)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductImageEntity> ApplyFilter(EntityQuery<ProductImageEntity> query, FilterCriteria filter)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductImageEntity> ApplySorting(EntityQuery<ProductImageEntity> query, string? sortBy, SortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductImageEntity> ApplyDefaultSorting(EntityQuery<ProductImageEntity> query)
    {
        throw new NotImplementedException();
    }

    protected override Task<IList<ProductImageEntity>> FetchEntitiesAsync(EntityQuery<ProductImageEntity> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductImage?>> GetByIdAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductImage>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductImage>> CreateAsync(ProductImage image, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(ProductImage image, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductImage?>> GetPrimaryImageByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> SetPrimaryImageAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RemovePrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductImage>>> GetImagesByTypeAsync(Guid productId, short imageType, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetImageCountByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}