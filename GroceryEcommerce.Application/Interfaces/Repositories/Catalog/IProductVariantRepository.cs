using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductVariantRepository
{
    // Basic CRUD operations
    Task<Result<ProductVariant?>> GetByIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductVariant>>> GetByProductIdAsync( PagedRequest request, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductVariant>>> GetBySkuAsync( PagedRequest request, string sku, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid variantId, CancellationToken cancellationToken = default);
    
    // Variant management operations
    Task<Result<bool>> ExistsAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariant?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductVariant>>> GetVariantsWithStockAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<int>> GetTotalStockByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductVariant>>> GetLowStockVariantsAsync( PagedRequest request, int threshold = 10, CancellationToken cancellationToken = default);
}
