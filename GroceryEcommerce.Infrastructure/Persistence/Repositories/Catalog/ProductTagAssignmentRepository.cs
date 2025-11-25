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

public class ProductTagAssignmentRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductTagAssignmentRepository> logger
) : BasePagedRepository<ProductTagAssignmentEntity, ProductTagAssignment>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductTagAssignmentRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "productid" => ProductTagAssignmentFields.ProductId,
            "tagid" => ProductTagAssignmentFields.TagId,
            _ => ProductTagAssignmentFields.ProductId
        };
    }
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField> {
            new SearchableField("ProductId", typeof(Guid)),
            new SearchableField("TagId", typeof(Guid)),
        };
    }

    public override string? GetDefaultSortField()
    {
        return "ProductId";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping> {
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TagId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase) {
            ["productid"] = ProductTagAssignmentFields.ProductId,
            ["tagid"] = ProductTagAssignmentFields.TagId
        };
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplySearch(EntityQuery<ProductTagAssignmentEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        
        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ProductTagAssignmentFields.ProductId,
            ProductTagAssignmentFields.TagId);

        return query.Where(predicate);
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplySorting(EntityQuery<ProductTagAssignmentEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductTagAssignmentEntity> ApplyDefaultSorting(EntityQuery<ProductTagAssignmentEntity> query)
    {
        return query.OrderBy(ProductTagAssignmentFields.ProductId.Ascending());
    }

    protected override async Task<IList<ProductTagAssignmentEntity>> FetchEntitiesAsync(EntityQuery<ProductTagAssignmentEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductTagAssignmentEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductTagAssignmentFields.ProductId;
    }

    protected override object GetEntityId(ProductTagAssignmentEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ProductId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    public async Task<Result<ProductTagAssignment?>> GetByIdAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty || tagId == Guid.Empty) {
            logger.LogWarning("Product id and tag id are required");
            return Result<ProductTagAssignment?>.Failure("Invalid product id or tag id.");
        }
        return await GetSingleAsync(ProductTagAssignmentFields.ProductId, productId, "ProductTagAssignment", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductTagAssignment>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("ProductId", productId), "ProductId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<PagedResult<ProductTagAssignment>>> GetByTagIdAsync(PagedRequest request, Guid tagId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, (r) => r.WithFilter("TagId", tagId), "ProductId", SortDirection.Ascending, cancellationToken);

    public async Task<Result<ProductTagAssignment>> CreateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductTagAssignmentEntity>(assignment);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync($"ProductTagAssignment_{entity.ProductId}_{entity.TagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByProduct_{entity.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByTag_{entity.TagId}", cancellationToken);
                logger.LogInformation("Product tag assignment created: {ProductId}, {TagId}", entity.ProductId, entity.TagId);
                return Result<ProductTagAssignment>.Success(Mapper.Map<ProductTagAssignment>(entity));
            }
            logger.LogWarning("Product tag assignment not created: {ProductId}, {TagId}", entity.ProductId, entity.TagId);
            return Result<ProductTagAssignment>.Failure("Product tag assignment not created.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error creating product tag assignment: {ProductId}, {TagId}", assignment.ProductId, assignment.TagId);
            return Result<ProductTagAssignment>.Failure("An error occurred while creating product tag assignment.", ex.Message);
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductTagAssignmentEntity>(assignment);
            entity.IsNew = false;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync($"ProductTagAssignment_{entity.ProductId}_{entity.TagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByProduct_{entity.ProductId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByTag_{entity.TagId}", cancellationToken);
                logger.LogInformation("Product tag assignment updated: {ProductId}, {TagId}", entity.ProductId, entity.TagId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product tag assignment not updated: {ProductId}, {TagId}", entity.ProductId, entity.TagId);
            return Result<bool>.Failure("Product tag assignment not updated.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating product tag assignment: {ProductId}, {TagId}", assignment.ProductId, assignment.TagId);
            return Result<bool>.Failure("An error occurred while updating product tag assignment.", ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        try {
            
            if (productId == Guid.Empty || tagId == Guid.Empty) {
                logger.LogWarning("Product id and tag id are required");
                return Result<bool>.Failure("Invalid product id or tag id.");
            }

            var query = new QueryFactory().
                ProductTagAssignment.Where(
                    ProductTagAssignmentFields.ProductId == productId &
                    ProductTagAssignmentFields.TagId == tagId
                );

            var adapter = GetAdapter();
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);

            if (entity is null) {
                logger.LogWarning("Product tag assignment not found: {ProductId}, {TagId}", productId, tagId);
                return Result<bool>.Failure("Product tag assignment not found.");
            }
            
            
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync($"ProductTagAssignment_{productId}_{tagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByProduct_{productId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByTag_{tagId}", cancellationToken);
                logger.LogInformation("Product tag assignment deleted: {ProductId}, {TagId}", productId, tagId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product tag assignment not deleted: {ProductId}, {TagId}", productId, tagId);
            return Result<bool>.Failure("Product tag assignment not deleted.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product tag assignment: {ProductId}, {TagId}", productId, tagId);
            return Result<bool>.Failure("An error occurred while deleting product tag assignment.", ex.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty || tagId == Guid.Empty) {
            logger.LogWarning("Product id and tag id are required");
            return Result<bool>.Failure("Invalid product id or tag id.");
        }
        return await ExistsByCountAsync(ProductTagAssignmentFields.ProductId, productId, cancellationToken);
    }

    public async Task<Result<bool>> AssignTagToProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductTagAssignmentEntity(productId, tagId);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync($"ProductTagAssignment_{productId}_{tagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByProduct_{productId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTagAssignments_ByTag_{tagId}", cancellationToken);
                logger.LogInformation("Product tag assignment assigned: {ProductId}, {TagId}", productId, tagId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product tag assignment not assigned: {ProductId}, {TagId}", productId, tagId);
            return Result<bool>.Failure("Product tag assignment not assigned.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error assigning product tag assignment: {ProductId}, {TagId}", productId, tagId);
            return Result<bool>.Failure("An error occurred while assigning product tag assignment.", ex.Message);
        }
    }

    public Task<Result<bool>> RemoveTagFromProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> RemoveAllTagsFromProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedResult<Guid>>> GetProductIdsByTagAsync(PagedRequest request, Guid tagId, CancellationToken cancellationToken = default)
    {
        try
        {
            request.WithFilter("TagId", tagId);
            if (!request.HasSorting)
            {
                request.WithSorting("ProductId", SortDirection.Ascending);
            }

            var qf = new QueryFactory();
            
            var countQuery = qf.Create()
                .From(qf.ProductTagAssignment)
                .Where(ProductTagAssignmentFields.TagId == tagId)
                .Select(Functions.CountRow());

            var adapter = GetAdapter();
            var totalCount = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);
            if (totalCount == 0)
            {
                logger.LogInformation("No product tag assignments found for tag: {TagId}", tagId);
                return Result<PagedResult<Guid>>.Success(new PagedResult<Guid>(new List<Guid>(), request.Page, request.PageSize, 0));
            }
            var query = qf.ProductTagAssignment
                .Where(ProductTagAssignmentFields.TagId == tagId)
                .Include(ProductTagAssignmentFields.ProductId);
            
            if (request.HasSorting)
            {
                query = ApplySorting(query, request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderBy(ProductTagAssignmentFields.ProductId.Ascending());
            }

            // Apply paging and select only ProductId
            query = query.Page(request.Page, request.PageSize);


            // Fetch as DynamicQuery to get only Guid values
            var entities = new EntityCollection<ProductTagAssignmentEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);

            var productIds = entities.Select(e => e.ProductId).ToList();

            logger.LogInformation("Product IDs by tag fetched: TagId {TagId}, Page {Page}, PageSize {PageSize}, Total {Total}", tagId, request.Page, request.PageSize, totalCount);
            
            return Result<PagedResult<Guid>>.Success(
                new PagedResult<Guid>(productIds, request.Page, request.PageSize, totalCount)
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching product IDs by tag: {TagId}", tagId);
            return Result<PagedResult<Guid>>.Failure("An error occurred while fetching product IDs by tag.");
        }
    }

    public async Task<Result<PagedResult<Guid>>> GetTagIdsByProductAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            request.WithFilter("ProductId", productId);
            if (!request.HasSorting)
            {
                request.WithSorting("TagId", SortDirection.Ascending);
            }

            var qf = new QueryFactory();
            var query = qf.ProductTagAssignment
                .Where(ProductTagAssignmentFields.ProductId == productId)
                .Include(ProductTagAssignmentFields.TagId);

            // Apply sorting
            if (request.HasSorting)
            {
                query = ApplySorting(query, request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderBy(ProductTagAssignmentFields.TagId.Ascending());
            }

            // Get total count
            var adapter = GetAdapter();
            var totalCount = await adapter.FetchScalarAsync<int>(
                query.Select(() => Functions.CountRow()),
                cancellationToken
            );

            // Apply paging and select TagId only
            query = query.Page(request.Page, request.PageSize);
                            

            var entities = new EntityCollection<ProductTagAssignmentEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);

            var tagIds = entities.Select(e => e.TagId).ToList();

            logger.LogInformation("Tag IDs by product fetched: ProductId {ProductId}, Page {Page}, PageSize {PageSize}, Total {Total}", productId, request.Page, request.PageSize, totalCount);

            return Result<PagedResult<Guid>>.Success(new PagedResult<Guid>(tagIds, request.Page, request.PageSize, totalCount));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching tag IDs by product: {ProductId}", productId);
            return Result<PagedResult<Guid>>.Failure("An error occurred while fetching tag IDs by product.");
        }
    }
}