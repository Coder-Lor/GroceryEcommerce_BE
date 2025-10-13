using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductRepository
{
    // Basic CRUD operations
    Task<Result<Product?>> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<Product?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid productId, CancellationToken cancellationToken = default);
    
    // Product management operations
    Task<Result<bool>> ExistsAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetByCategoryIdAsync( PagedRequest request, Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetByBrandIdAsync( PagedRequest request, Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetFeaturedProductsAsync( PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetActiveProductsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetLowStockProductsAsync( PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> SearchProductsAsync( PagedRequest request, string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Product>>> GetProductsByPriceRangeAsync( PagedRequest request, decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStatusAsync(Guid productId, short status, CancellationToken cancellationToken = default);
}