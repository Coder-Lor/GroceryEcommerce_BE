using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IPurchaseOrderItemRepository
{
    // Basic CRUD operations
    Task<Result<PurchaseOrderItem?>> GetByIdAsync(Guid poiId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrderItem>>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrderItem>>> GetByProductIdAsync(Guid productId, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<bool> CreateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid poiId, CancellationToken cancellationToken = default);
    
    // Purchase order item management operations
    Task<Result<bool>> ExistsAsync(Guid poiId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalAmountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrderItem>>> GetByVariantIdAsync(Guid variantId, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
}
