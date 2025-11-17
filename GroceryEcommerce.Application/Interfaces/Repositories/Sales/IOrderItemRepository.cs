using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderItemRepository : IPagedRepository<OrderItem>
{
    // Basic CRUD operations
    Task<Result<OrderItem?>> GetByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderItem>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderItem>>> GetByProductIdAsync(Guid productId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(OrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    
    // Order item management operations
    Task<Result<bool>> ExistsAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalAmountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderItem>>> GetByVariantIdAsync(Guid variantId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderItem>>> GetBySkuAsync(string sku, PagedRequest request, CancellationToken cancellationToken = default);
}
