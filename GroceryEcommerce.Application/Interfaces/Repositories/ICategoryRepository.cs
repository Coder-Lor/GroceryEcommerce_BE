using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    // Basic CRUD operations
    Task<Result<Category?>> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<Category?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<Category?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Category>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Category>> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid categoryId, CancellationToken cancellationToken = default);
    
    // Category hierarchy operations
    Task<Result<List<Category>>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> GetCategoryPathAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> GetCategoryTreeAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> HasSubCategoriesAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsRootCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    
    // Category management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Category>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsCategoryInUseAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductCountByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
