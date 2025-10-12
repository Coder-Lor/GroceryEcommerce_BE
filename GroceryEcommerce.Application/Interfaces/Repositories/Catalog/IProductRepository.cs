using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductRepository : IPagedRepository<Product>
{
    // Basic CRUD operations
    Task<Result<PagedResult<Product>>> GetProductsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
    
    // Product management operations
    Task<Result<bool>> ExistsAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<List<Product>>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStatusAsync(Guid productId, short status, CancellationToken cancellationToken = default);
}