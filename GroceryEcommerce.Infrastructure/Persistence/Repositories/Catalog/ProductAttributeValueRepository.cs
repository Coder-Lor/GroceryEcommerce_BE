using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class ProductAttributeValueRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductAttributeValueRepository> logger
) : BasePagedRepository<ProductAttributeValueEntity, ProductAttributeValue>(adapter, mapper, cacheService, logger), IProductAttributeValueRepository
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

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplySearch(EntityQuery<ProductAttributeValueEntity> query, string searchTerm)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplySorting(EntityQuery<ProductAttributeValueEntity> query, string? sortBy, SortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductAttributeValueEntity> ApplyDefaultSorting(EntityQuery<ProductAttributeValueEntity> query)
    {
        throw new NotImplementedException();
    }

    protected override Task<IList<ProductAttributeValueEntity>> FetchEntitiesAsync(EntityQuery<ProductAttributeValueEntity> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductAttributeValue?>> GetByIdAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedResult<ProductAttributeValue>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedResult<ProductAttributeValue>>> GetByAttributeIdAsync(PagedRequest request, Guid attributeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductAttributeValue>> CreateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(Guid valueId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductAttributeValue?>> GetByProductAndAttributeAsync(Guid productId, Guid attributeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteByAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedResult<ProductAttributeValue>>> GetByValueAsync(PagedRequest request, string value, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}