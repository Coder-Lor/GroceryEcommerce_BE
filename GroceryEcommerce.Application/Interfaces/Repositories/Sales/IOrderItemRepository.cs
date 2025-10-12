using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderItemRepository
{
    // Basic CRUD operations
    Task<Result<OrderItem?>> GetByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderItem>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderItem>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<OrderItem>> CreateAsync(OrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    
    // Order item management operations
    Task<Result<bool>> ExistsAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalAmountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderItem>>> GetByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderItem>>> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
}
