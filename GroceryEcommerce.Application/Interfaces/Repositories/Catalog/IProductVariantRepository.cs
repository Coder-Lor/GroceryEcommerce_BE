using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductVariantRepository
{
    // Basic CRUD operations
    Task<Result<ProductVariant?>> GetByIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductVariant>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariant>> CreateAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid variantId, CancellationToken cancellationToken = default);
    
    // Variant management operations
    Task<Result<bool>> ExistsAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariant?>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<List<ProductVariant>>> GetVariantsWithStockAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockAsync(Guid variantId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<int>> GetTotalStockByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductVariant>>> GetLowStockVariantsAsync(int threshold = 10, CancellationToken cancellationToken = default);
}
