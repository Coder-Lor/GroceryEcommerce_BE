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

public class CategoryRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<CategoryRepository> logger)
    : BasePagedRepository<CategoryEntity, Category>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), ICategoryRepository
{
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
            new SearchableField("DisplayOrder", typeof(int))
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
            new FieldMapping { FieldName = "Name", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Description", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "Slug", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "DisplayOrder", FieldType = typeof(int), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "Name", CategoryFields.Name },
            { "Description", CategoryFields.Description },
            { "Slug", CategoryFields.Slug },
            { "Status", CategoryFields.Status },
            { "CreatedAt", CategoryFields.CreatedAt },
            { "DisplayOrder", CategoryFields.DisplayOrder }
        };
    }

    protected override EntityQuery<CategoryEntity> ApplySearch(EntityQuery<CategoryEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;
        searchTerm = searchTerm.Trim().ToLower();

        return query.Where(
            CategoryFields.Name.Contains(searchTerm) |
            CategoryFields.Slug.Contains(searchTerm) |
            CategoryFields.Description.Contains(searchTerm)
        );
    }
    
    protected override EntityQuery<CategoryEntity> ApplySorting(EntityQuery<CategoryEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var sortField = GetSortField(sortBy);
        if (sortField is null) return query;

        return sortDirection == SortDirection.Descending
            ? query.OrderBy(sortField.Descending())
            : query.OrderBy(sortField.Ascending());
    }

    protected override EntityQuery<CategoryEntity> ApplyDefaultSorting(EntityQuery<CategoryEntity> query)
    {
        return query.OrderBy(CategoryFields.Name.Ascending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return CategoryFields.CategoryId;
    }

    protected override object GetEntityId(CategoryEntity entity, EntityField2 primaryKeyField)
    {
        return entity.CategoryId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return new PredicateExpression(primaryKeyField.In(ids));
    }

    private EntityField2? GetSortField(string? sortBy)
    {
        return sortBy?.ToLower() switch
        {
            "name" => CategoryFields.Name,
            "slug" => CategoryFields.Slug,
            "createdat" => CategoryFields.CreatedAt,
            "displayorder" => CategoryFields.DisplayOrder,
            "status" => CategoryFields.Status,
            _ => CategoryFields.Name
        };
    }
    
    public async Task<Result<Category?>> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                Logger.LogWarning("Attempted to fetch category with empty GUID.");
                return Result<Category?>.Failure("Invalid category ID.");
            }
            var cacheKey = $"Category_{categoryId}";
            var cachedCategory = await CacheService.GetAsync<Category>(cacheKey, cancellationToken);
            if (cachedCategory != null)
            {
                Logger.LogInformation("Category fetched from cache: {CategoryId}", categoryId);
                return Result<Category?>.Success(cachedCategory);
            }

            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == categoryId);
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var categoryEntity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (categoryEntity == null)
            {
                Logger.LogInformation("Category not found: {CategoryId}", categoryId);
                return Result<Category?>.Failure("Category not found.");
            }
            var category = Mapper.Map<Category>(categoryEntity);
            await CacheService.SetAsync(cacheKey, category, TimeSpan.FromHours(1), cancellationToken);
                Logger.LogInformation("Category fetched from database and cached: {CategoryId}", categoryId);
            return Result<Category?>.Success(category);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching category by ID: {CategoryId}", categoryId);
            return Result<Category?>.Failure("An error occurred while fetching the category.");
        }
    }

    public async Task<Result<Category?>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("name of category is required");
                return Result<Category?>.Failure("Invalid category name");
            }
            
            var cacheKey = $"Category_Name_{name}";
            var cachedCategory = await CacheService.GetAsync<Category>(cacheKey, cancellationToken);
            if (cachedCategory != null)
            {
                logger.LogInformation("Category fetched from cache: {Name}", name);
                return Result<Category?>.Success(cachedCategory);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.Name == name);
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var categoryEntity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (categoryEntity == null)
            {
                logger.LogInformation("Category not found: {Name}", name);
                return Result<Category?>.Failure("Category not found.");
            }
            var category = Mapper.Map<Category>(categoryEntity);
            await CacheService.SetAsync(cacheKey, category, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Category fetched from database and cached: {Name}", name);
            return Result<Category?>.Success(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching category by name: {Name}", name);
            return Result<Category?>.Failure("An errorr occurred while fetching the category.");
        }
    }

    public async Task<Result<Category?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                logger.LogWarning("slug of category is required");
                return Result<Category?>.Failure("Invalid category slug");
            }
            
            var cacheKey = $"Category_Slug_{slug}";
            
            var cachedCategory = await CacheService.GetAsync<Category>(cacheKey, cancellationToken);
            if (cachedCategory != null)
            {
                logger.LogInformation("Category fetched from cache: {Slug}", slug);
                return Result<Category?>.Success(cachedCategory);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.Slug == slug);
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var categoryEntity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (categoryEntity == null)
            {
                logger.LogInformation("Category not found: {Slug}", slug);
                return Result<Category?>.Failure("Category not found.");
            }
            var category = Mapper.Map<Category>(categoryEntity);
            await CacheService.SetAsync(cacheKey, category, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Category fetched from database and cached: {Slug}", slug);
            return Result<Category?>.Success(category);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching category by slug: {Slug}", slug);
            return Result<Category?>.Failure("An error occurred while fetching the category.");
        }
    }

    public async Task<Result<List<Category>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = "All_Categories";
            var cachedCategories = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cachedCategories != null)
            {
                logger.LogInformation("All categories fetched from cache.");
                return Result<List<Category>>.Success(cachedCategories);
            }

            var qf = new QueryFactory();
            var query = qf.Category
                .OrderBy(CategoryFields.Name.Descending());
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var categoryEntities = await adapter.FetchQueryAsync(query, cancellationToken);
            var categories = Mapper.Map<List<Category>>(categoryEntities);
            await CacheService.SetAsync(cacheKey, categories, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("All categories fetched from database and cached.");
            return Result<List<Category>>.Success(categories);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching all categories");
            return Result<List<Category>>.Failure("An error occurred while fetching all categories.");
        }
    }
    
    public async Task<Result<Category>> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<CategoryEntity>(category);
            entity.IsNew = true;
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync("All_Categories", cancellationToken);
                await CacheService.RemoveAsync("Root_Categories", cancellationToken);
                
                logger.LogInformation("Category created: {Name}", category.Name);
                return Result<Category>.Success(category);
            }
            logger.LogWarning("Failed to create category: {Name}", category.Name);
            return Result<Category>.Failure("Failed to create category.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating category: {Name}", category.Name);
            return Result<Category>.Failure("An error occurred while creating category.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = Mapper.Map<CategoryEntity>(category);
            entity.IsNew = false;
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var saved = await adapter.SaveEntityAsync(entity, cancellationToken);
            if (saved)
            {
                await CacheService.RemoveAsync("All_Categories", cancellationToken);
                await CacheService.RemoveAsync("Root_Categories", cancellationToken);
                
                logger.LogInformation("Category updated: {Name}", category.Name);
                return Result<bool>.Success(saved);
            }
            logger.LogWarning("Failed to update category: {Name}", category.Name);
            return Result<bool>.Failure("Failed to update category.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating category: {Name}", category.Name);
            return Result<bool>.Failure("An error occurred while updating category.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<bool>.Failure("Invalid category ID.");
            }
            var entity = new CategoryEntity(categoryId);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var deleted = await adapter.DeleteEntityAsync(entity, cancellationToken);
            if (deleted)
            {
                await CacheService.RemoveAsync("All_Categories", cancellationToken);
                await CacheService.RemoveAsync("Root_Categories", cancellationToken);
                
                logger.LogInformation("Category deleted: {CategoryId}", categoryId);
                return Result<bool>.Success(deleted);
            }
            logger.LogWarning("Failed to delete category: {CategoryId}", categoryId);
            return Result<bool>.Failure("Failed to delete category.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting category: {CategoryId}", categoryId);
            return Result<bool>.Failure("An error occurred while deleting category.");
        }
    }

    public async Task<Result<List<Category>>> GetRootCategoriesAsync(CancellationToken ct = default)
    {
        try
        {
            const string cacheKey = "Root_Categories";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, ct);
            if (cached != null)
            {
                logger.LogInformation("Root categories fetched from cache");
                return Result<List<Category>>.Success(cached);
            }

            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == Guid.Empty)
                .OrderBy(CategoryFields.Name.Ascending());
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, ct);
            var result = Mapper.Map<List<Category>>(entities);

            await CacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1), ct);
            return Result<List<Category>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting root categories");
            return Result<List<Category>>.Failure("An error occurred while retrieving root categories");
        }
    }

    public async Task<Result<List<Category>>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (parentCategoryId == Guid.Empty)
            {
                logger.LogWarning("Parent category id is required");
                return Result<List<Category>>.Failure("Invalid parent category ID.");
            }
            
            var cacheKey = $"Sub_Categories_{parentCategoryId}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Sub categories fetched from cache: {ParentCategoryId}", parentCategoryId);
                return Result<List<Category>>.Success(cached);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.ParentCategoryId == parentCategoryId)
                .OrderBy(CategoryFields.Name.Ascending());
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            if (entities.Count == 0) return Result<List<Category>>.Success(new List<Category>());
            
            var result = Mapper.Map<List<Category>>(entities);
            if (result.Count == 0)
            {
                logger.LogInformation("No sub categories found for parent category: {ParentCategoryId}", parentCategoryId);
                return Result<List<Category>>.Success(new List<Category>());
            }
            
            logger.LogInformation("Sub categories fetched for parent category: {ParentCategoryId}", parentCategoryId);
            return Result<List<Category>>.Success(result);
            
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting sub categories");
            return Result<List<Category>>.Failure("An error occurred while retrieving sub categories");
        }
    }

    public async Task<Result<List<Category>>> GetCategoryPathAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<List<Category>>.Failure("Invalid category ID.");
            }
            
            var cacheKey = $"Category_Path_{categoryId}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Category path fetched from cache: {CategoryId}", categoryId);
                return Result<List<Category>>.Success(cached);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == categoryId)
                .OrderBy(CategoryFields.Name.Ascending());
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            if (entities.Count == 0) return Result<List<Category>>.Success(new List<Category>());
            
            var result = Mapper.Map<List<Category>>(entities);
            if (result.Count == 0)
            {
                logger.LogInformation("No category path found for category: {CategoryId}", categoryId);
                return Result<List<Category>>.Success(new List<Category>());
            }
            logger.LogInformation("Category path fetched for category: {CategoryId}", categoryId);
            return Result<List<Category>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category path");
            return Result<List<Category>>.Failure("An error occurred while retrieving category path");
        }
    }

    public async Task<Result<List<Category>>> GetCategoryTreeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // var cacheKey = "Category_Tree";
            // var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            // if (cached != null)
            // {
            //     logger.LogInformation("Category tree fetched from cache");
            //     return Result<List<Category>>.Success(cached);
            // }
            
            var level1 = (IPrefetchPathElement2)CategoryEntity.PrefetchPathCategories;
            level1.Sorter = new SortExpression(CategoryFields.Name.Ascending());
            
            var level2 = (IPrefetchPathElement2)level1.SubPath.Add(CategoryEntity.PrefetchPathCategories);
            level2.Sorter = new SortExpression(CategoryFields.Name.Ascending());
            
            var level3 = (IPrefetchPathElement2)level2.SubPath.Add(CategoryEntity.PrefetchPathCategories);
            level3.Sorter = new SortExpression(CategoryFields.Name.Ascending());
            
            var qf = new QueryFactory();

            var query = qf.Category
                .Where(CategoryFields.ParentCategoryId.IsNull())
                .WithPath(level1)
                .OrderBy(CategoryFields.Name.Ascending());
            
            var entities = new EntityCollection<CategoryEntity>();
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            await adapter.FetchQueryAsync(query, entities, cancellationToken);
            var result = Mapper.Map<List<Category>>(entities);
            if (result.Count == 0)
            {
                logger.LogInformation("No category tree found");
                return Result<List<Category>>.Success(new List<Category>());
            }
            // await CacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Category tree fetched");
            return Result<List<Category>>.Success(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting category tree");
            return Result<List<Category>>.Failure("An error occurred while retrieving category tree");
        }
    }

    public async Task<Result<bool>> HasSubCategoriesAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<bool>.Failure("Invalid category ID.");
            }
            var cacheKey = $"Categories_{categoryId}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null && cached.Count > 0)
            {
                logger.LogInformation("Category has sub categories (from cache): {CategoryId}", categoryId);
                return Result<bool>.Success(true);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.ParentCategoryId == categoryId);
                
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) return Result<bool>.Success(false);
            
            logger.LogInformation("Category has sub categories: {CategoryId}", categoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category has sub categories");
            return Result<bool>.Failure("An error occurred while checking if category has sub categories");
        }
    }

    public async Task<Result<bool>> IsRootCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<bool>.Failure("Invalid category ID.");
            }
            var cacheKey = $"Categories_{categoryId}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null && cached.Count > 0)
            {
                logger.LogInformation("Category is root category (from cache): {CategoryId}", categoryId);
                return Result<bool>.Success(true);           
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == categoryId);
                
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) return Result<bool>.Success(false);
            
            logger.LogInformation("Category is root category: {CategoryId}", categoryId);
            return Result<bool>.Success(entity.ParentCategoryId == Guid.Empty);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category is root category");
            return Result<bool>.Failure("An error occurred while checking if category is root category");            
        }
    }

    public async Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Category name is required");
                return Result<bool>.Failure("Invalid category name.");
            }
            var cacheKey = $"Category_Name_{name}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null && cached.Count > 0)
            {
                logger.LogInformation("Category exists (from cache): {Name}", name);
                return Result<bool>.Success(true);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.Name == name);
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) return Result<bool>.Success(false);
            logger.LogInformation("Category exists: {Name}", name);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category exists by name: {Name}", name);
            return Result<bool>.Failure("An error occurred while checking if category exists.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<bool>.Failure("Invalid category ID.");
            }
            var cacheKey = $"Category_{categoryId}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null && cached.Count > 0)
            {
                logger.LogInformation("Category exists (from cache): {CategoryId}", categoryId);
                return Result<bool>.Success(true);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == categoryId);
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null) return Result<bool>.Success(false);
            logger.LogInformation("Category exists: {CategoryId}", categoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category exists by id: {CategoryId}", categoryId);
            return Result<bool>.Failure("An error occurred while checking if category exists.");       
        }
    }

    public async Task<Result<(bool ExitsByName, bool ExitsById)>> ExistsAsync(string name, Guid categoryId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Category name is required");
                return Result<(bool ExitsByName, bool ExitsById)>.Failure("Invalid category name.");
            }
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<(bool ExitsByName, bool ExitsById)>.Failure("Invalid category ID.");
            }
            
            var cachedById = $"Category_{categoryId}";
            var cachedId = await CacheService.GetAsync<Category>(cachedById, cancellationToken);
            if (cachedId != null)
            {
                logger.LogInformation("Category exists (from cache): {Name}, {CategoryId}", name, categoryId);
                return Result<(bool ExitsByName, bool ExitsById)>.Success((false, true));
            }
            
            var cachedByName = $"Category_Name_{name}";
            var cachedName = await CacheService.GetAsync<Category>(cachedByName, cancellationToken);
            if (cachedName != null)
            {
                logger.LogInformation("Category exists (from cache): {Name}, {CategoryId}", name, categoryId);
                return Result<(bool ExitsByName, bool ExitsById)>.Success((true, false));
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.CategoryId == categoryId | CategoryFields.Name == name)
                .Limit(2);
            var entities = new EntityCollection<CategoryEntity>();
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            await adapter.FetchQueryAsync(query, entities, cancellationToken);

            var hasName = entities.Any(e => e.Name == name);
            var hasId = entities.Any(e => e.CategoryId == categoryId);
            logger.LogInformation("Category exists: {Name}, {CategoryId}", name, categoryId);
            return Result<(bool ExitsByName, bool ExitsById)>.Success((hasName, hasId));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category exists by name and id: {Name}, {CategoryId}", name, categoryId);
            return Result<(bool ExitsByName, bool ExitsById)>.Failure("An error occurred while checking if category exists.");       
        }
    }

    public async Task<Result<List<Category>>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = "Active_Categories";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Active categories fetched from cache");
                return Result<List<Category>>.Success(cached);
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.Status == 1)
                .OrderBy(CategoryFields.Name.Ascending());
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var result = Mapper.Map<List<Category>>(entities);
            if (result.Count == 0)
            {
                logger.LogInformation("No active categories found");
                return Result<List<Category>>.Success(new List<Category>());
            }
            
            await CacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Active categories fetched");
            return Result<List<Category>>.Success(result);       
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting active categories");
            return Result<List<Category>>.Failure("An error occurred while retrieving active categories.");       
        }
    }

    public async Task<Result<List<Category>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                logger.LogWarning("Search term is required");
                return Result<List<Category>>.Failure("Invalid search term.");
            }
            
            var cacheKey = $"Categories_Name_{searchTerm}";
            var cached = await CacheService.GetAsync<List<Category>>(cacheKey, cancellationToken);
            if (cached != null)
            {
                logger.LogInformation("Categories searched by name (from cache): {SearchTerm}", searchTerm);
                return Result<List<Category>>.Success(cached);           
            }
            
            var qf = new QueryFactory();
            var query = qf.Category
                .Where(CategoryFields.Name.Contains(searchTerm))
                .OrderBy(CategoryFields.Name.Ascending());
            
            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var result = Mapper.Map<List<Category>>(entities);
            if (result.Count == 0)
            {
                logger.LogInformation("No categories found by name: {SearchTerm}", searchTerm);
                return Result<List<Category>>.Success(new List<Category>());
            }
            
            await CacheService.SetAsync(cacheKey, result, TimeSpan.FromHours(1), cancellationToken);
            logger.LogInformation("Categories searched by name: {SearchTerm}", searchTerm);
            return Result<List<Category>>.Success(result);       
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching categories by name: {SearchTerm}", searchTerm);
            return Result<List<Category>>.Failure("An error occurred while searching categories by name.");       
        }
    }

    public async Task<Result<bool>> IsCategoryInUseAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<bool>.Failure("Invalid category ID.");
            }

            var adapter = GetAdapter();
            var qf = new QueryFactory();

            // 1. Check category status trước (optional nếu bạn cần Status == 1)
            var catQuery = qf.Category
                .Where(CategoryFields.CategoryId == categoryId)
                .Select(CategoryFields.Status);

            var categoryStatus = await adapter.FetchScalarAsync<int?>(catQuery, cancellationToken);

            if (categoryStatus is null)
            {
                logger.LogWarning("Category not found: {CategoryId}", categoryId);
                return Result<bool>.Failure("Category not found or inactive.");
            }

            // 2. Đếm product thuộc category đó
            // giả sử ProductFields.CategoryId là FK
            var productCountQuery = qf.Product
                .Where(ProductFields.CategoryId == categoryId & ProductFields.Status == 1);

            var entity = await adapter.FetchFirstAsync(productCountQuery);

            bool inUse = entity != null;

            return Result<bool>.Success(inUse);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if category is in use: {CategoryId}", categoryId);
            return Result<bool>.Failure("An error occurred while checking if category is in use.");
        }
    }


    public async Task<Result<int>> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (categoryId == Guid.Empty)
            {
                logger.LogWarning("Category id is required");
                return Result<int>.Failure("Invalid category ID.");
            }

            var qf = new QueryFactory();
            var query = qf.Product
                .Where(ProductFields.CategoryId == categoryId)
                .Select(() => Functions.CountRow());

            var adapter = GetAdapter(); // Sử dụng adapter phù hợp
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            logger.LogInformation("Product count fetched for category: {CategoryId}, Count: {Count}", categoryId, count);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting product count by category: {CategoryId}", categoryId);
            return Result<int>.Failure("An error occurred while retrieving product count by category.");
        }
    }
}