using GroceryEcommerce.Application.Common.Interfaces;

namespace GroceryEcommerce.Application.Catalog.Interfaces
{
    public interface ICategoryRepository : IRepository<CategoryEntity>
    {
        Task<IEnumerable<CategoryEntity>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryEntity>> GetChildCategoriesAsync(Guid parentId, CancellationToken cancellationToken = default);
        Task<CategoryEntity?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryEntity>> GetCategoryHierarchyAsync(Guid categoryId, CancellationToken cancellationToken = default);
    }
}