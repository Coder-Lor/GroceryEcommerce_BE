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

public class ProductImageRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductImageRepository> logger
) : BasePagedRepository<ProductImageEntity, ProductImage>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductImageRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "alttext" => ProductImageFields.AltText,
            "createdat" => ProductImageFields.CreatedAt,
            "displayorder" => ProductImageFields.DisplayOrder,
            "imageid" => ProductImageFields.ImageId,
            "imageurl" => ProductImageFields.ImageUrl,
            "isprimary" => ProductImageFields.IsPrimary,
            "productid" => ProductImageFields.ProductId,
            _ => ProductImageFields.DisplayOrder
        };
    }

    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
			new SearchableField("AltText", typeof(string)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("DisplayOrder", typeof(int)),
            new SearchableField("ImageId", typeof(Guid)),
            new SearchableField("ImageUrl", typeof(string)),
            new SearchableField("IsPrimary", typeof(bool)),
            new SearchableField("ProductId", typeof(Guid)),
        };
    }

    public override string? GetDefaultSortField()
    {
        return "DisplayOrder";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "AltText", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DisplayOrder", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ImageId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ImageUrl", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "IsPrimary", FieldType = typeof(bool), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ProductId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true, IsFilterable = true },
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>
        {
            ["alttext"] = ProductImageFields.AltText,
            ["createdat"] = ProductImageFields.CreatedAt,
            ["displayorder"] = ProductImageFields.DisplayOrder,
            ["imageid"] = ProductImageFields.ImageId,
            ["imageurl"] = ProductImageFields.ImageUrl,
            ["isprimary"] = ProductImageFields.IsPrimary,
            ["productid"] = ProductImageFields.ProductId,
        };
    }

    protected override EntityQuery<ProductImageEntity> ApplySearch(EntityQuery<ProductImageEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        return query.Where(
            ProductImageFields.AltText.Contains(searchTerm) |
            ProductImageFields.ImageUrl.Contains(searchTerm) |
            ProductImageFields.IsPrimary.Contains(searchTerm) |
            ProductImageFields.ProductId.Contains(searchTerm)
        );
    }

    protected override EntityQuery<ProductImageEntity> ApplySorting(EntityQuery<ProductImageEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy) ?? ProductImageFields.DisplayOrder;
        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(sortField.Ascending())
            : query.OrderBy(sortField.Descending());
    }

    protected override EntityQuery<ProductImageEntity> ApplyDefaultSorting(EntityQuery<ProductImageEntity> query)
    {
        return query.OrderBy(ProductImageFields.DisplayOrder.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductImageFields.ImageId;
    }

    protected override object GetEntityId(ProductImageEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ImageId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    protected override async Task<IList<ProductImageEntity>> FetchEntitiesAsync(EntityQuery<ProductImageEntity> query, DataAccessAdapter adapter, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductImageEntity>();
        await adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }

    public Task<Result<ProductImage?>> GetByIdAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        if (imageId == Guid.Empty)
        {
            logger.LogWarning("Image id is required");
            return Task.FromResult(Result<ProductImage?>.Failure("Invalid image ID."));
        }
        return GetSingleAsync(ProductImageFields.ImageId, imageId, "ProductImage", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<PagedResult<ProductImage>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, request => request.WithFilter("productid", productId, FilterOperator.Equals), "imageid", SortDirection.Ascending, cancellationToken);

    public async Task<Result<List<string>>> GetImageUrlsByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");
            return Result<List<string>>.Failure("Invalid product ID.");
        }

        try
        {
            var qf = new QueryFactory();
            var query = qf.ProductImage
                .Where(ProductImageFields.ProductId == productId)
                // Ưu tiên ảnh chính trước, sau đó theo thứ tự hiển thị
                .OrderBy(
                    ProductImageFields.IsPrimary.Descending(),
                    ProductImageFields.DisplayOrder.Ascending(),
                    ProductImageFields.ImageId.Ascending()
                );

            var adapter = GetAdapter();
            var entities = new EntityCollection<ProductImageEntity>();
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var urls = entities.Select(e => e.ImageUrl).ToList();
            return Result<List<string>>.Success(urls);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching image urls for product: {ProductId}", productId);
            return Result<List<string>>.Failure("An error occurred while fetching image urls.");
        }
    }

    public async Task<Result<ProductImage>> CreateAsync(ProductImage image, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductImageEntity>(image);
            entity.IsNew = true;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductImages", cancellationToken);
                await CacheService.RemoveAsync($"ProductImage_{image.ImageId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductImages_ByProduct_{image.ProductId}", cancellationToken);
                logger.LogInformation("Product image created: {Image}", image);
            
                // Trả về image gốc vì đã được save thành công
                return Result<ProductImage>.Success(image);
            }
            logger.LogWarning("Product image not created: {ImageId}", image.ImageId);
            return Result<ProductImage>.Failure("Product image not created.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error creating product image: {ImageId}", image.ImageId);
            return Result<ProductImage>.Failure("An error occurred while creating product image.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(ProductImage image, CancellationToken cancellationToken = default)
    {
        try {
            var entity = Mapper.Map<ProductImageEntity>(image);
            entity.IsNew = false;
            var adapter = GetAdapter();
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved) {
                await CacheService.RemoveAsync("All_ProductImages", cancellationToken);
                await CacheService.RemoveAsync($"ProductImage_{image.ImageId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductImages_ByProduct_{image.ProductId}", cancellationToken);
                logger.LogInformation("Product image updated: {Image}", image);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product image not updated: {ImageId}", image.ImageId);
            return Result<bool>.Failure("Product image not updated.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error updating product image: {ImageId}", image.ImageId);
            return Result<bool>.Failure("An error occurred while updating product image.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        try {
            var entity = new ProductImageEntity(imageId);
            var adapter = GetAdapter();
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted) {
                await CacheService.RemoveAsync("All_ProductImages", cancellationToken);
                await CacheService.RemoveAsync($"ProductImage_{imageId}", cancellationToken);
                await CacheService.RemoveAsync($"ProductImages_ByProduct_{imageId}", cancellationToken);
                logger.LogInformation("Product image deleted: {ImageId}", imageId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product image not deleted: {ImageId}", imageId);
            return Result<bool>.Failure("Product image not deleted.");
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error deleting product image: {ImageId}", imageId);
            return Result<bool>.Failure("An error occurred while deleting product image.");
        }
    }

    public Task<Result<bool>> ExistsAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        if (imageId == Guid.Empty)
        {
            logger.LogWarning("Image id is required");
            return Task.FromResult(Result<bool>.Failure("Invalid image ID."));
        }
        return ExistsByCountAsync(ProductImageFields.ImageId, imageId, cancellationToken);
    }

    public Task<Result<ProductImage?>> GetPrimaryImageByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");
            return Task.FromResult(Result<ProductImage?>.Failure("Invalid product ID."));
        }
        return GetSingleAsync(ProductImageFields.ProductId, productId, "ProductImage", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<bool>> SetPrimaryImageAsync(Guid imageId, CancellationToken cancellationToken = default)
    {
        if (imageId == Guid.Empty)
        {
            logger.LogWarning("Image id is required");
            return Result<bool>.Failure("Invalid image ID.");
        }
        
        try
        {
            var query = new QueryFactory().ProductImage
                .Where(ProductImageFields.ImageId == imageId);
            
            var adapter = GetAdapter();
            var imageEntity = await adapter.FetchFirstAsync(query, cancellationToken);

            if (imageEntity == null)
            {
                logger.LogWarning("Image not found: {ImageId}", imageId);
                return Result<bool>.Failure("Image not found.");
            }

            var resetTemplate = new ProductImageEntity { IsPrimary = false };
            resetTemplate.Fields[nameof(ProductImageEntity.IsPrimary)].IsChanged = true;

            await adapter.UpdateEntitiesDirectlyAsync(
                resetTemplate,
                new RelationPredicateBucket(ProductImageFields.ProductId == imageEntity.ProductId),
                cancellationToken
            );
            
            var setPrimaryTemplate = new ProductImageEntity { IsPrimary = true };
            setPrimaryTemplate.Fields[nameof(ProductImageEntity.IsPrimary)].IsChanged = true;
        
            await adapter.UpdateEntitiesDirectlyAsync(
                setPrimaryTemplate,
                new RelationPredicateBucket(ProductImageFields.ImageId == imageId),
                cancellationToken
            );

            // Clear cache
            await CacheService.RemoveAsync("All_ProductImages", cancellationToken);
            await CacheService.RemoveAsync($"ProductImage_{imageId}", cancellationToken);
            await CacheService.RemoveAsync($"ProductImages_ByProduct_{imageEntity.ProductId}", cancellationToken);
        
            logger.LogInformation("Primary image set: {ImageId} for product {ProductId}", imageId, imageEntity.ProductId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting primary image: {ImageId}", imageId);
            return Result<bool>.Failure("An error occurred while setting the primary image.");
        }
    }

    public async Task<Result<bool>> RemovePrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");           
            return Result<bool>.Failure("Invalid product ID.");       
        }
        try {
            var template = new ProductImageEntity { IsPrimary = false };
            template.Fields["IsPrimary"].IsChanged = true;

            var adapter = GetAdapter();
            var affectedRows = await adapter.UpdateEntitiesDirectlyAsync(
                template,
                new RelationPredicateBucket(ProductImageFields.ProductId == productId),
                cancellationToken           
            );
            
            if (affectedRows > 0) {
                await CacheService.RemoveAsync("All_ProductImages", cancellationToken);
                await CacheService.RemoveAsync($"ProductImages_ByProduct_{productId}", cancellationToken);
                logger.LogInformation("Primary image removed for product: {ProductId}", productId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("No primary image found for product: {ProductId}", productId);
            return Result<bool>.Success(false);           
        }
        catch (Exception ex) {
            logger.LogError(ex, "Error removing primary image: {ProductId}", productId);
            return Result<bool>.Failure("An error occurred while removing the primary image.");
        }
    }

    public async Task<Result<PagedResult<ProductImage>>> GetImagesByTypeAsync(PagedRequest request, Guid productId, short imageType,
        CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(
            request, 
            r => r.WithFilter("ProductId", productId, FilterOperator.Equals)
            .WithFilter("ImageType", imageType, FilterOperator.Equals),
            "ImageId", 
            SortDirection.Ascending, 
            cancellationToken
        );

    public async Task<Result<int>> GetImageCountByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");           
            return Result<int>.Failure("Invalid product ID.");       
        }
        return await CountByFieldAsync(ProductImageFields.ProductId, productId, cancellationToken);
    }
}