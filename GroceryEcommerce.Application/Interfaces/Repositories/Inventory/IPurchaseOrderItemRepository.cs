using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IPurchaseOrderItemRepository
{
    // Basic CRUD operations
    Task<Result<PurchaseOrderItem?>> GetByIdAsync(Guid poiId, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderItem>>> GetByPurchaseOrderIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderItem>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderItem>> CreateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid poiId, CancellationToken cancellationToken = default);
    
    // Purchase order item management operations
    Task<Result<bool>> ExistsAsync(Guid poiId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalAmountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByPurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderItem>>> GetByVariantIdAsync(Guid variantId, CancellationToken cancellationToken = default);
}
