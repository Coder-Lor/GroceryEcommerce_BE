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

public class ProductTagRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductTagRepository> logger
) : BasePagedRepository<ProductTagEntity, ProductTag>(adapter, mapper, cacheService, logger), IProductTagRepository
{

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
			"name" => ProductTagFields.Name,
			"slug" => ProductTagFields.Slug,
			"tagid" => ProductTagFields.TagId,
			_ => ProductTagFields.Name
        };
    }
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField> {
            new SearchableField("Name", typeof(string)),
            new SearchableField("Slug", typeof(string)),
            new SearchableField("TagId", typeof(Guid)),
        };
    }

    public override string? GetDefaultSortField()
    {
        return "Name";
    }
    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping> {
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Slug", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TagId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase) {
            ["name"] = ProductTagFields.Name,
            ["slug"] = ProductTagFields.Slug,
            ["tagid"] = ProductTagFields.TagId,
        };
    }

    protected override EntityQuery<ProductTagEntity> ApplySearch(EntityQuery<ProductTagEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        return query.Where(
            ProductTagFields.Name.Contains(searchTerm) |
            ProductTagFields.Slug.Contains(searchTerm) |
            ProductTagFields.TagId.Contains(searchTerm)
        );
    }

    protected override EntityQuery<ProductTagEntity> ApplySorting(EntityQuery<ProductTagEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? ProductTagFields.Name;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductTagEntity> ApplyDefaultSorting(EntityQuery<ProductTagEntity> query)
    {
        return query.OrderBy(ProductTagFields.Name.Ascending());
    }

    protected override async Task<IList<ProductTagEntity>> FetchEntitiesAsync(EntityQuery<ProductTagEntity> query, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductTagEntity>();
        await Adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public async Task<Result<ProductTag?>> GetByIdAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        if (tagId == Guid.Empty)
        {
            logger.LogWarning("Tag id is required");
            return Result<ProductTag?>.Failure("Invalid tag ID.");
        }
        return await GetSingleAsync(ProductTagFields.TagId, tagId, "ProductTag", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<ProductTag?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogWarning("Tag name is required");
            return Result<ProductTag?>.Failure("Invalid tag name.");
        }
        return await GetSingleAsync(ProductTagFields.Name, name, "ProductTag", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<ProductTag?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Tag slug is required");
            return Result<ProductTag?>.Failure("Invalid tag slug.");
        }
        return await GetSingleAsync(ProductTagFields.Slug, slug, "ProductTag", TimeSpan.FromHours(1), cancellationToken);
    }

    public Task<Result<List<ProductTag>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProductTag>> CreateAsync(ProductTag tag, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductTagEntity>(tag);
            entity.IsNew = true;
            var saved = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductTags", cancellationToken);
                await CacheService.RemoveAsync($"ProductTag_{entity.TagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_ByName_{entity.Name}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_BySlug_{entity.Slug}", cancellationToken);
                logger.LogInformation("Tag created: {Name}", tag.Name);
                return Result<ProductTag>.Success(Mapper.Map<ProductTag>(entity));
            }
            logger.LogWarning("Tag not created: {Name}", tag.Name);
            return Result<ProductTag>.Failure("Tag not created.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error creating tag");
            return Result<ProductTag>.Failure("An error occurred while creating tag.", ex.Message);
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductTag tag, CancellationToken cancellationToken = default)
    {
        try {
            
            var entity = await Adapter.FetchFirstAsync(
                new QueryFactory().ProductTag.Where(ProductTagFields.TagId == tag.TagId),
                cancellationToken
            );
        
            if (entity == null) {
                logger.LogWarning("Tag not found: {TagId}", tag.TagId);
                return Result<bool>.Failure("Tag not found.");
            }
            Mapper.Map(tag, entity);
            entity.IsNew = false;
            var saved = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductTags", cancellationToken);
                await CacheService.RemoveAsync($"ProductTag_{entity.TagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_ByName_{entity.Name}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_BySlug_{entity.Slug}", cancellationToken);
            }
            logger.LogWarning("Tag not updated: {Name}", tag.Name);
            return Result<bool>.Success(saved);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating tag");
            return Result<bool>.Failure("An error occurred while updating tag.", ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (tagId == Guid.Empty)
            {
                logger.LogWarning("Tag id is required");
                return Result<bool>.Failure("Invalid tag ID.");
            }
            var entity = await Adapter.FetchFirstAsync(
                new QueryFactory().ProductTag.Where(ProductTagFields.TagId == tagId),
                cancellationToken
            );
            if (entity == null)
            {
                logger.LogWarning("Tag not found: {TagId}", tagId);
                return Result<bool>.Failure("Tag not found.");
            }
            var deleted = await Adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync("All_ProductTags", cancellationToken);
                await CacheService.RemoveAsync($"ProductTag_{entity.TagId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_ByName_{entity.Name}", cancellationToken);
                await CacheService.RemoveAsync($"ProductTags_BySlug_{entity.Slug}", cancellationToken);
                logger.LogInformation("Tag deleted: {TagId}", tagId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Tag not deleted: {TagId}", tagId);
            return Result<bool>.Failure("Tag not deleted.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting tag");
            return Result<bool>.Failure("An error occurred while deleting tag.", ex.Message);
        }
    }

    public async Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogWarning("Tag name is required");
            return Result<bool>.Failure("Invalid tag name.");
        }
        return await ExistsByCountAsync(ProductTagFields.Name, name, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        if (tagId == Guid.Empty)
        {
            logger.LogWarning("Tag id is required");
            return Result<bool>.Failure("Invalid tag ID.");
        }
        return await ExistsByCountAsync(ProductTagFields.TagId, tagId, cancellationToken);
    }

    public async Task<Result<PagedResult<ProductTag>>> SearchByNameAsync(PagedRequest request, string searchTerm, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Name", searchTerm), cancellationToken: cancellationToken);

    public async Task<Result<bool>> IsTagInUseAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        try {
            if (tagId == Guid.Empty)
            {
                logger.LogWarning("Tag id is required");
                return Result<bool>.Failure("Invalid tag ID.");
            }
            
            return await ExistsByCountAsync(ProductTagAssignmentFields.TagId, tagId, cancellationToken);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error checking if tag is in use");
            return Result<bool>.Failure("An error occurred while checking if tag is in use.", ex.Message);
        }
    }

    public async Task<Result<int>> GetProductCountByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        try {
            if (tagId == Guid.Empty)
            {
                logger.LogWarning("Tag id is required");
                return Result<int>.Failure("Invalid tag ID.");
            }
            return await CountByFieldAsync(ProductTagAssignmentFields.TagId, tagId, cancellationToken);
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error getting product count by tag");
            return Result<int>.Failure("An error occurred while getting product count by tag.", ex.Message);
        }
    }
}