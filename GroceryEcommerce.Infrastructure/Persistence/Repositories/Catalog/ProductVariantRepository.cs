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

public class ProductVariantRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductVariantRepository> logger
) : BasePagedRepository<ProductVariantEntity, ProductVariant>(adapter, mapper, cacheService, logger), IProductVariantRepository
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

    protected override EntityQuery<ProductVariantEntity> ApplySearch(EntityQuery<ProductVariantEntity> query, string searchTerm)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductVariantEntity> ApplyFilter(EntityQuery<ProductVariantEntity> query, FilterCriteria filter)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductVariantEntity> ApplySorting(EntityQuery<ProductVariantEntity> query, string? sortBy, SortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductVariantEntity> ApplyDefaultSorting(EntityQuery<ProductVariantEntity> query)
    {
        throw new NotImplementedException();
    }

    protected override Task<IList<ProductVariantEntity>> FetchEntitiesAsync(EntityQuery<ProductVariantEntity> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductVariant?>> GetByIdAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductVariant>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductVariant>> CreateAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(Guid variantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductVariant?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductVariant>>> GetVariantsWithStockAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<int>> GetTotalStockByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductVariant>>> GetLowStockVariantsAsync(int threshold = 10, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}