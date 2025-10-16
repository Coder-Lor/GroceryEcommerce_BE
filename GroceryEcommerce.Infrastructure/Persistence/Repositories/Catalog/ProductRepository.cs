using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Catalog;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Catalog;

public class ProductRepository(
    DataAccessAdapter adapter,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductRepository> logger
    ): BasePagedRepository<ProductEntity, Product>(adapter, mapper, cacheService, logger), IProductRepository
{
    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => ProductFields.Name,
            "price" => ProductFields.Price,
            "createdat" => ProductFields.CreatedAt,
            "updatedat" => ProductFields.UpdatedAt,
            _ => null
        };
    }
    
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("Name", typeof(string)),
            new SearchableField("Description", typeof(string)),
            new SearchableField("Slug", typeof(string)),
            new SearchableField("MetaTitle", typeof(string)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("CreatedAt", typeof(DateTime)),
            new SearchableField("DisplayOrder", typeof(int)),
            new SearchableField("Price", typeof(decimal)),
            new SearchableField("Stock", typeof(int)),
            new SearchableField("Sku", typeof(string)),
            new SearchableField("CategoryId", typeof(Guid)),
            new SearchableField("BrandId", typeof(Guid)),
            new SearchableField("IsFeatured", typeof(bool))
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
                FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Description", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Slug", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "MetaTitle", FieldType = typeof(string), IsSearchable = true, IsSortable = false,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "DisplayOrder", FieldType = typeof(int), IsSearchable = false, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Price", FieldType = typeof(decimal), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Stock", FieldType = typeof(int), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "Sku", FieldType = typeof(string), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "CategoryId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            },
            new FieldMapping
            {
                FieldName = "BrandId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
                IsFilterable = true
            }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>()
        {
            ["name"] = ProductFields.Name,
            ["slug"] = ProductFields.Slug,
            ["status"] = ProductFields.Status,
            ["createdat"] = ProductFields.CreatedAt,
            ["updatedat"] = ProductFields.UpdatedAt,
            ["sku"] = ProductFields.Sku,
            ["price"] = ProductFields.Price,
            ["stock"] = ProductFields.StockQuantity,
            ["categoryid"] = ProductFields.CategoryId,
            ["brandid"] = ProductFields.BrandId,
            ["isfeatured"] = ProductFields.IsFeatured
        };
    }

    protected override EntityQuery<ProductEntity> ApplySearch(EntityQuery<ProductEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();
        
        return query.Where(
            ProductFields.Name.Contains(searchTerm) |
            ProductFields.MetaTitle.Contains(searchTerm) |
            ProductFields.Slug.Contains(searchTerm)
        );
    }
    
    protected override EntityQuery<ProductEntity> ApplySorting(EntityQuery<ProductEntity> query, string? sortBy, SortDirection sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;
        
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;
        
        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<ProductEntity> ApplyDefaultSorting(EntityQuery<ProductEntity> query)
    {
        return query.OrderBy(ProductFields.Name.Ascending());   
    }

    protected override async Task<IList<ProductEntity>> FetchEntitiesAsync(EntityQuery<ProductEntity> query, CancellationToken cancellationToken)
    {
        var entities = new EntityCollection<ProductEntity>();
        await Adapter.FetchQueryAsync(query, entities, cancellationToken);
        return entities;
    }


    public async Task<Result<Product?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");
            return Result<Product?>.Failure("Invalid product ID.");   
        }
        return await GetSingleAsync(ProductFields.ProductId, productId, "Product", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<Product?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            logger.LogWarning("Product sku is required");
            return Result<Product?>.Failure("Invalid product SKU.");  
        }
        return await GetSingleAsync(ProductFields.Sku, sku, "Product_Sku", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<Product?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Product slug is required");
            return Result<Product?>.Failure("Invalid product slug."); 
        }
        return await GetSingleAsync(ProductFields.Slug, slug, "Product_Slug", TimeSpan.FromHours(1), cancellationToken);
    }

    public async Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {   
            var entity = Mapper.Map<ProductEntity>(product);
            entity.IsNew = true;
            
            var saved = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync("All_Products", cancellationToken);
                logger.LogInformation("Product created: {Name}", product.Name);
                return Result<Product>.Success(product);
            }
            logger.LogWarning("Product not created: {Name}", product.Name);
            return Result<Product>.Failure("Product not created.");       
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating product: {Name}", product.Name);
            return Result<Product>.Failure("An error occurred while creating product.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<ProductEntity>(product);
            entity.IsNew = false;
            
            var updated = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated)
            {
                await CacheService.RemoveAsync("All_Products", cancellationToken);
                logger.LogInformation("Product updated: {Name}", product.Name);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Product not updated: {Name}", product.Name);
            return Result<bool>.Failure("Product not updated.");      
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating product: {Name}", product.Name);
            return Result<bool>.Failure("An error occurred while updating product.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Invalid product ID.");
            }
            
            var entity = new ProductEntity(productId);
            var deleted = await Adapter.DeleteEntityAsync(entity, cancellationToken);

            if (!deleted)
            {
                logger.LogWarning("Product not deleted: {ProductId}", productId);
                return Result<bool>.Failure("Product not deleted.");
            }
            logger.LogInformation("Product deleted: {ProductId}", productId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting product: {ProductId}", productId);
            return Result<bool>.Failure("An error occurred while deleting product.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            logger.LogWarning("Product sku is required");
            return Result<bool>.Failure("Invalid product SKU."); 
        }
        return await ExistsByCountAsync(ProductFields.Sku, sku, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");
            return Result<bool>.Failure("Invalid product ID.");
        }
        return await ExistsByCountAsync(ProductFields.ProductId, productId, cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetByCategoryIdAsync(PagedRequest request, Guid categoryId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("CategoryId", categoryId), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetByBrandIdAsync(PagedRequest request, Guid brandId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("BrandId", brandId), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetFeaturedProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("IsFeatured", true), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetActiveProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Status", 1), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetLowStockProductsAsync(PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Stock", threshold, FilterOperator.LessThanOrEqual), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> SearchProductsAsync(PagedRequest request, string searchTerm, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithSearch(searchTerm), cancellationToken: cancellationToken);

    public Task<Result<PagedResult<Product>>> GetProductsByPriceRangeAsync(PagedRequest request, decimal minPrice,
        decimal maxPrice, CancellationToken cancellationToken = default)
        => GetPagedConfiguredAsync(
            request, 
            r => r.WithRangeFilter("Price", minPrice, maxPrice), 
            GetDefaultSortField(), SortDirection.Ascending, cancellationToken
        );

    public async Task<Result<bool>> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Invalid product ID.");
            }
            
            var entity = new ProductEntity(productId);
            entity.StockQuantity = quantity;
            
            var updated = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated)
            {
                logger.LogInformation("Stock updated for product: {ProductId}", productId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Stock not updated for product: {ProductId}", productId);
            return Result<bool>.Failure("Stock not updated.");      
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating stock");
            return Result<bool>.Failure("An error occurred while updating stock.");      
        }
    }

    public async Task<Result<bool>> UpdateStatusAsync(Guid productId, short status, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Invalid product ID.");
            }
            
            var entity = new ProductEntity(productId)
            {
                Status = status
            };

            var updated = await Adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated)
            {
                logger.LogInformation("Status updated for product: {ProductId}", productId);
                return Result<bool>.Success(true);
            }
            logger.LogWarning("Status not updated for product: {ProductId}", productId);
            return Result<bool>.Failure("Status not updated.");      
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating status");
            return Result<bool>.Failure("An error occurred while updating status.");      
        }
    }
}