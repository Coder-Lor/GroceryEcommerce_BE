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

public class ProductTagAssignmentRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductTagAssignmentRepository> logger
) : BasePagedRepository<ProductTagAssignmentEntity, ProductTagAssignment>(adapter, mapper, cacheService, logger), IProductTagAssignmentRepository
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

    protected override EntityQuery<ProductTagAssignmentEntity> ApplySearch(EntityQuery<ProductTagAssignmentEntity> query, string searchTerm)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplyFilter(EntityQuery<ProductTagAssignmentEntity> query, FilterCriteria filter)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplySorting(EntityQuery<ProductTagAssignmentEntity> query, string? sortBy, SortDirection sortDirection)
    {
        throw new NotImplementedException();
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplyDefaultSorting(EntityQuery<ProductTagAssignmentEntity> query)
    {
        throw new NotImplementedException();
    }

    protected override Task<IList<ProductTagAssignmentEntity>> FetchEntitiesAsync(EntityQuery<ProductTagAssignmentEntity> query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductTagAssignment?>> GetByIdAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductTagAssignment>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<ProductTagAssignment>>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<ProductTagAssignment>> CreateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ExistsAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> AssignTagToProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RemoveTagFromProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RemoveAllTagsFromProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Guid>>> GetProductIdsByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Guid>>> GetTagIdsByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}