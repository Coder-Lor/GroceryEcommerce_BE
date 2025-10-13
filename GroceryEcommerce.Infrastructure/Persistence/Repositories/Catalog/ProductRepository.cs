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
            new SearchableField("Name", typeof(string), true, true),
            new SearchableField("Description", typeof(string), true, false),
            new SearchableField("Slug", typeof(string), true, false),
            new SearchableField("MetaTitle", typeof(string), true, false),
            new SearchableField("Status", typeof(short), false, true),
            new SearchableField("CreatedAt", typeof(DateTime), false, true),
            new SearchableField("DisplayOrder", typeof(int), false, true),
            new SearchableField("Price", typeof(decimal), false, true),
            new SearchableField("Stock", typeof(int), false, true),
            new SearchableField("Sku", typeof(string), false, true),
            new SearchableField("CategoryId", typeof(Guid), false, true),
            new SearchableField("BrandId", typeof(Guid), false, true)
        };
    }

    public override string? GetDefaultSortField()
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

    protected override EntityQuery<ProductEntity> ApplyFilter(EntityQuery<ProductEntity> query, FilterCriteria filter)
    {
        return filter.Operator switch
        {
            FilterOperator.Equals when filter.FieldName.Equals("name", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Name == filter.Value.ToString()),

            FilterOperator.Contains when filter.FieldName.Equals("name", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Name.Contains(filter.Value.ToString())),

            FilterOperator.Equals when filter.FieldName.Equals("slug", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Slug == filter.Value.ToString()),

            FilterOperator.Contains when filter.FieldName.Equals("slug", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Slug.Contains(filter.Value.ToString())),

            FilterOperator.Equals when filter.FieldName.Equals("status", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Status == Convert.ToInt16(filter.Value)),

            FilterOperator.GreaterThan when filter.FieldName.Equals("createdat", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.CreatedAt > Convert.ToDateTime(filter.Value)),

            FilterOperator.LessThan when filter.FieldName.Equals("createdat", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.CreatedAt < Convert.ToDateTime(filter.Value)),

            FilterOperator.GreaterThan when filter.FieldName.Equals("updatedat", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.UpdatedAt > Convert.ToDateTime(filter.Value)),

            FilterOperator.LessThan when filter.FieldName.Equals("updatedat", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.UpdatedAt < Convert.ToDateTime(filter.Value)),

            FilterOperator.Equals when filter.FieldName.Equals("sku", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Sku == filter.Value.ToString()),

            FilterOperator.Contains when filter.FieldName.Equals("sku", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Sku.Contains(filter.Value.ToString())),

            FilterOperator.Equals when filter.FieldName.Equals("price", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Price == Convert.ToDecimal(filter.Value)),

            FilterOperator.GreaterThan when filter.FieldName.Equals("price", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Price > Convert.ToDecimal(filter.Value)),

            FilterOperator.LessThan when filter.FieldName.Equals("price", StringComparison.OrdinalIgnoreCase) =>
                query.Where(ProductFields.Price < Convert.ToDecimal(filter.Value)),

            _ => query
        };
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
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<Product?>.Failure("Invalid product ID.");
            }
            
            var cacheKey = $"Product_{productId}";
            var cached = await cacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache: {ProductId}", productId);
                return Result<Product?>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.ProductId == productId);

            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found: {ProductId}", productId);
                return Result<Product?>.Failure("Product not found.");
            }
            var product = mapper.Map<Product>(entity);
            await cacheService.SetAsync(cacheKey, product, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached: {ProductId}", productId);
            return Result<Product?>.Success(product);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product by id: {ProductId}", productId);
            return Result<Product?>.Failure("An error occurred while fetching the product.");       
        }
    }

    public async Task<Result<Product?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                logger.LogWarning("Product sku is required");
                return Result<Product?>.Failure("Invalid product SKU.");
            }
            var cacheKey = $"Product_Sku_{sku}";
            var cached = await cacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache: {Sku}", sku);
                return Result<Product?>.Success(cached);
            }
            
            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.Sku == sku);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found: {Sku}", sku);
                return Result<Product?>.Failure("Product not found.");
            }
            var product = mapper.Map<Product>(entity);
            await cacheService.SetAsync(cacheKey, product, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached: {Sku}", sku);
            return Result<Product?>.Success(product);   
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product by sku: {Sku}", sku);
            return Result<Product?>.Failure("An error occurred while fetching the product.");      
        }
    }

    public async Task<Result<Product?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                logger.LogWarning("Product slug is required");
                return Result<Product?>.Failure("Invalid product slug.");
            }
            
            var cacheKey = $"Product_Slug_{slug}";
            var cached = await cacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache: {Slug}", slug);
                return Result<Product?>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.Slug == slug);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found: {Slug}", slug);
                return Result<Product?>.Failure("Product not found.");
            }
            var product = mapper.Map<Product>(entity);
            await cacheService.SetAsync(cacheKey, product, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached: {Slug}", slug);
            return Result<Product?>.Success(product);  
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product by slug: {Slug}", slug);
            return Result<Product?>.Failure("An error occurred while fetching the product.");      
        }
    }

    public async Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {   
            var entity = mapper.Map<ProductEntity>(product);
            entity.IsNew = true;
            
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await cacheService.RemoveAsync("All_Products", cancellationToken);
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
            var entity = mapper.Map<ProductEntity>(product);
            entity.IsNew = false;
            
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (updated)
            {
                await cacheService.RemoveAsync("All_Products", cancellationToken);
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
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);

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
        try
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                logger.LogWarning("Product sku is required");
                return Result<bool>.Failure("Invalid product SKU.");
            }
            
            var cacheKey = $"Product_Sku_{sku}";
            var cached = await cacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product existence fetched from cache: {Sku}", sku);
                return Result<bool>.Success(true);
            }
            
            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.Sku == sku);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogInformation("Product not found: {Sku}", sku);
                return Result<bool>.Success(false);
            }
            logger.LogInformation("Product existence fetched from database: {Sku}", sku);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if product exists: {Sku}", sku);
            return Result<bool>.Failure("An error occurred while checking if product exists.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Invalid product ID.");
            }
            
            var cacheKey = $"Product_{productId}";
            var cached = await cacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product existence fetched from cache: {ProductId}", productId);
                return Result<bool>.Success(true);
            }
            
            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.ProductId == productId);
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogInformation("Product not found: {ProductId}", productId);
                return Result<bool>.Success(false);
            }
            logger.LogInformation("Product existence fetched from database: {ProductId}", productId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if product exists: {ProductId}", productId);
            return Result<bool>.Failure("An error occurred while checking if product exists.");
        }
    }

    public async Task<Result<PagedResult<Product>>> GetByCategoryIdAsync(PagedRequest request, Guid categoryId, CancellationToken cancellationToken = default) 
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("CategoryId", categoryId, FilterOperator.Equals), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetByBrandIdAsync(PagedRequest request, Guid brandId, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("BrandId", brandId, FilterOperator.Equals), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetFeaturedProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("IsFeatured", true, FilterOperator.Equals), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetActiveProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Status", 1, FilterOperator.Equals), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetLowStockProductsAsync(PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithFilter("Stock", threshold, FilterOperator.LessThanOrEqual), cancellationToken: cancellationToken);
    
    public async Task<Result<PagedResult<Product>>> SearchProductsAsync(PagedRequest request, string searchTerm, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(request, r => r.WithSearch(searchTerm), cancellationToken: cancellationToken);

    public async Task<Result<PagedResult<Product>>> GetProductsByPriceRangeAsync(PagedRequest request, decimal minPrice,
        decimal maxPrice, CancellationToken cancellationToken = default)
        => await GetPagedConfiguredAsync(
            request, 
    r => { r.AddFilter("Price", minPrice, FilterOperator.GreaterThanOrEqual); 
            r.AddFilter("Price", maxPrice, FilterOperator.LessThanOrEqual); }, 
            GetDefaultSortField() ?? "Name", SortDirection.Ascending, cancellationToken: cancellationToken
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
            
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
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

            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
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