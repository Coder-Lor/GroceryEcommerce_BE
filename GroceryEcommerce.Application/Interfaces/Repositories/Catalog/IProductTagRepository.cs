using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductTagRepository
{
    // Basic CRUD operations
    Task<Result<ProductTag?>> GetByIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<ProductTag?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<ProductTag?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<List<ProductTag>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductTag>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductTag>> CreateAsync(ProductTag tag, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductTag tag, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid tagId, CancellationToken cancellationToken = default);
    
    // Tag management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductTag>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsTagInUseAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductCountByTagAsync(Guid tagId, CancellationToken cancellationToken = default);
}
