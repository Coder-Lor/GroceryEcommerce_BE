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
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<ProductRepository> logger
    ): BasePagedRepository<ProductEntity, Product>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IProductRepository
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
            new SearchableField("ShopId", typeof(Guid)),
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
            },
            new FieldMapping
            {
                FieldName = "ShopId", FieldType = typeof(Guid), IsSearchable = true, IsSortable = true,
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
            ["shopid"] = ProductFields.ShopId,
            ["isfeatured"] = ProductFields.IsFeatured
        };
    }

    protected override EntityQuery<ProductEntity> ApplySearch(EntityQuery<ProductEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            ProductFields.Name,
            ProductFields.MetaTitle,
            ProductFields.Slug);

        return query.Where(predicate);
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


    protected PrefetchPath2 BuildProductPrefetchPath()
    {
        var prefetchPath = new PrefetchPath2(EntityType.ProductEntity);
        
        // Thêm prefetch cho ProductImages
        prefetchPath.Add(ProductEntity.PrefetchPathProductImages);
        
        // Thêm prefetch cho ProductVariants
        prefetchPath.Add(ProductEntity.PrefetchPathProductVariants);
        
        // Thêm prefetch cho ProductTagAssignments và nested ProductTag
        var productTagAssignmentsPath = ProductEntity.PrefetchPathProductTagAssignments;
        productTagAssignmentsPath.SubPath.Add(ProductTagAssignmentEntity.PrefetchPathProductTag);
        prefetchPath.Add(productTagAssignmentsPath);
        
        return prefetchPath;
    }

    protected IReadOnlyList<IPrefetchPathElement2> BuildProductPrefetchPathElements()
    {
        var prefetchPaths = new List<IPrefetchPathElement2>();

        // Thêm prefetch cho ProductImages
        prefetchPaths.Add(ProductEntity.PrefetchPathProductImages);

        // Thêm prefetch cho ProductVariants
        prefetchPaths.Add(ProductEntity.PrefetchPathProductVariants);

        // Thêm prefetch cho ProductTagAssignments và nested ProductTag
        var productTagAssignmentsPath = ProductEntity.PrefetchPathProductTagAssignments;
        productTagAssignmentsPath.SubPath.Add(ProductTagAssignmentEntity.PrefetchPathProductTag);
        prefetchPaths.Add(productTagAssignmentsPath);

        return prefetchPaths;
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return ProductFields.ProductId;
    }

    protected override object GetEntityId(ProductEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ProductId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }


    public async Task<Result<Product?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty)
        {
            logger.LogWarning("Product id is required");
            return Result<Product?>.Failure("Invalid product ID.");   
        }
        
        try
        {
            var cacheKey = $"Product_{productId}";
            var cached = await CacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache by ID: {ProductId}", productId);
                return Result<Product?>.Success(cached);
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<ProductEntity>()
                .Where(ProductFields.ProductId == productId);

            foreach (var path in BuildProductPrefetchPathElements())
            {
                query = query.WithPath(path);
            }
                
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found by ID: {ProductId}", productId);
                return Result<Product?>.Failure("Product not found.");
            }

            var domainEntity = Mapper.Map<Product>(entity);
            await CacheService.SetAsync(cacheKey, domainEntity, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached by ID: {ProductId}", productId);
            return Result<Product?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Product by ID: {ProductId}", productId);
            return Result<Product?>.Failure("An error occurred while fetching Product.");
        }
    }

    public async Task<Result<Product?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            logger.LogWarning("Product sku is required");
            return Result<Product?>.Failure("Invalid product SKU.");  
        }
        
        try
        {
            var cacheKey = $"Product_Sku_{sku}";
            var cached = await CacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache by SKU: {Sku}", sku);
                return Result<Product?>.Success(cached);
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<ProductEntity>()
                .Where(ProductFields.Sku == sku);

            foreach (var path in BuildProductPrefetchPathElements())
            {
                query = query.WithPath(path);
            }
                
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found by SKU: {Sku}", sku);
                return Result<Product?>.Failure("Product not found.");
            }

            var domainEntity = Mapper.Map<Product>(entity);
            await CacheService.SetAsync(cacheKey, domainEntity, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached by SKU: {Sku}", sku);
            return Result<Product?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Product by SKU: {Sku}", sku);
            return Result<Product?>.Failure("An error occurred while fetching Product.");
        }
    }

    public async Task<Result<Product?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            logger.LogWarning("Product slug is required");
            return Result<Product?>.Failure("Invalid product slug."); 
        }
        
        try
        {
            var cacheKey = $"Product_Slug_{slug}";
            var cached = await CacheService.GetAsync<Product>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Product fetched from cache by slug: {Slug}", slug);
                return Result<Product?>.Success(cached);
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<ProductEntity>()
                .Where(ProductFields.Slug == slug);

            foreach (var path in BuildProductPrefetchPathElements())
            {
                query = query.WithPath(path);
            }
                
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Product not found by slug: {Slug}", slug);
                return Result<Product?>.Failure("Product not found.");
            }

            var domainEntity = Mapper.Map<Product>(entity);
            //await CacheService.SetAsync(cacheKey, domainEntity, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Product fetched from database and cached by slug: {Slug}", slug);
            return Result<Product?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Product by slug: {Slug}", slug);
            return Result<Product?>.Failure("An error occurred while fetching Product.");
        }
    }

    public async Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {   
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = Mapper.Map<ProductEntity>(product);
            entity.IsNew = true;
            
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
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
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = Mapper.Map<ProductEntity>(product);
            entity.IsNew = false;
            
            var updated = await adapter.SaveEntityAsync(entity, cancellationToken);
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
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
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
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithFilter("CategoryId", categoryId), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetByBrandIdAsync(PagedRequest request, Guid brandId, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithFilter("BrandId", brandId), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetByShopIdAsync(PagedRequest request, Guid shopId, CancellationToken cancellationToken = default)
    {
        if (shopId == Guid.Empty)
        {
            logger.LogWarning("Shop id is required");
            return Result<PagedResult<Product>>.Failure("Invalid shop ID.");
        }

        try
        {
            request.AvailableFields = GetSearchableFields();
            var validation = request.Validate();
            if (validation != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                logger.LogWarning("Invalid paged request: {Errors}", validation?.ErrorMessage);
                return Result<PagedResult<Product>>.Failure(validation?.ErrorMessage ?? "Invalid paged request");
            }

            var prefetchPath = BuildProductPrefetchPath();
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            // Filter trực tiếp bằng EntityField thay vì qua FilterCriteria để tránh lỗi parse Guid
            var query = qf.Create<ProductEntity>()
                .Where(ProductFields.ShopId == shopId);
            var countQuery = qf.Create<ProductEntity>()
                .Where(ProductFields.ShopId == shopId);

            // Apply search
            if (request.HasSearch)
            {
                query = ApplySearch(query, request.Search ?? string.Empty);
                countQuery = ApplySearch(countQuery, request.Search ?? string.Empty);
            }

            // Apply other filters (excluding ShopId since we already filtered it)
            if (request.HasFilters)
            {
                foreach (var filter in request.Filters.Where(f => !f.FieldName.Equals("ShopId", StringComparison.OrdinalIgnoreCase)))
                {
                    query = ApplyFilter(query, filter);
                    countQuery = ApplyFilter(countQuery, filter);
                }
            }

            // Get total count
            var countEntities = await FetchEntitiesAsync(countQuery, adapter, null, cancellationToken);
            var totalCount = countEntities.Count;

            // Apply sorting
            if (request.HasSorting)
            {
                query = ApplySorting(query, request.SortBy, request.SortDirection);
            }
            else
            {
                query = ApplyDefaultSorting(query);
            }

            // Apply paging
            query = query.Page(request.Page, request.PageSize);

            // Fetch data với PrefetchPath
            var entities = await FetchEntitiesAsync(query, adapter, prefetchPath, cancellationToken);
            var domainEntities = Mapper.Map<List<Product>>(entities);

            var result = new PagedResult<Product>(domainEntities, totalCount, request.Page, request.PageSize);

            logger.LogInformation("Paged Products by ShopId fetched: ShopId {ShopId}, Page {Page}, PageSize {PageSize}, Total {Total}", 
                shopId, request.Page, request.PageSize, totalCount);

            return Result<PagedResult<Product>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products by shop: {ShopId}", shopId);
            return Result<PagedResult<Product>>.Failure($"An error occurred while fetching products by shop.");
        }
    }

    public async Task<Result<PagedResult<Product>>> GetFeaturedProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithFilter("IsFeatured", true), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetActiveProductsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithFilter("Status", 1), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetLowStockProductsAsync(PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithFilter("Stock", threshold, FilterOperator.LessThanOrEqual), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> SearchProductsAsync(PagedRequest request, string searchTerm, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(request, r => r.WithSearch(searchTerm), prefetchPath, cancellationToken: cancellationToken);
    }

    public async Task<Result<PagedResult<Product>>> GetProductsByPriceRangeAsync(PagedRequest request, decimal minPrice,
        decimal maxPrice, CancellationToken cancellationToken = default)
    {
        var prefetchPath = BuildProductPrefetchPath();
        return await GetPagedConfiguredAsync(
            request, 
            r => r.WithRangeFilter("Price", minPrice, maxPrice), 
            prefetchPath,
            GetDefaultSortField(), SortDirection.Ascending, cancellationToken
        );
    }

    public async Task<Result<PagedResult<Product>>> GetProductsByTagNameAsync(PagedRequest request, string tagName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                logger.LogWarning("Tag name is required");
                return Result<PagedResult<Product>>.Failure("Invalid tag name.");
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            // Tìm tag theo name
            var tagQuery = qf.Create<ProductTagEntity>()
                .Where(ProductTagFields.Name == tagName);
            var tag = await adapter.FetchFirstAsync(tagQuery, cancellationToken);
            
            if (tag == null)
            {
                logger.LogInformation("No products found for tag name: {TagName}", tagName);
                return Result<PagedResult<Product>>.Success(new PagedResult<Product>(new List<Product>(), request.Page, request.PageSize, 0));
            }

            // Sử dụng Exists để filter products có tag này
            var prefetchPath = BuildProductPrefetchPath();
            
            // Override GetPagedAsync để filter bằng Exists
            var productQuery = qf.Create<ProductEntity>()
                .Where(ProductFields.ProductId.In(
                    qf.ProductTagAssignment
                        .Where(ProductTagAssignmentFields.TagId == tag.TagId)
                        .Select(ProductTagAssignmentFields.ProductId)
                ));

            // Apply filters từ request
            if (request.HasFilters)
            {
                foreach (var filter in request.Filters)
                {
                    productQuery = ApplyFilter(productQuery, filter);
                }
            }

            // Get total count
            var countQuery = productQuery.Select(() => Functions.CountRow());
            var totalCount = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);

            if (totalCount == 0)
            {
                return Result<PagedResult<Product>>.Success(new PagedResult<Product>(new List<Product>(), request.Page, request.PageSize, 0));
            }

            // Apply sorting
            if (request.HasSorting)
            {
                productQuery = ApplySorting(productQuery, request.SortBy, request.SortDirection);
            }
            else
            {
                productQuery = ApplyDefaultSorting(productQuery);
            }

            // Apply paging
            productQuery = productQuery.Page(request.Page, request.PageSize);

            // Fetch entities với prefetch path
            var entities = await FetchEntitiesAsync(productQuery, adapter, prefetchPath, cancellationToken);
            var domainEntities = Mapper.Map<List<Product>>(entities);

            var result = new PagedResult<Product>(domainEntities, totalCount, request.Page, request.PageSize);
            logger.LogInformation("Products by tag name fetched: TagName {TagName}, Page {Page}, PageSize {PageSize}, Total {Total}", 
                tagName, request.Page, request.PageSize, totalCount);
            
            return Result<PagedResult<Product>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching products by tag name: {TagName}", tagName);
            return Result<PagedResult<Product>>.Failure("An error occurred while fetching products by tag name.");
        }
    }

    public async Task<Result<bool>> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        try
        {
            if (productId == Guid.Empty)
            {
                logger.LogWarning("Product id is required");
                return Result<bool>.Failure("Invalid product ID.");
            }
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
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
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
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