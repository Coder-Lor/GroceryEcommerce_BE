using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IBrandRepository
{
    // Basic CRUD operations
    Task<Result<Brand?>> GetByIdAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<Brand?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<Brand?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<List<Brand>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Brand>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Brand>> CreateAsync(Brand brand, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Brand brand, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid brandId, CancellationToken cancellationToken = default);
    
    // Brand management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<List<Brand>>> GetActiveBrandsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Brand>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsBrandInUseAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductCountByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
}
