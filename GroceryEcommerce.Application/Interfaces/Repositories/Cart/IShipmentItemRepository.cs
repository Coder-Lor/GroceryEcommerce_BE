using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Cart;

public interface IShipmentItemRepository
{
    // Basic CRUD operations
    Task<Result<ShipmentItem?>> GetByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ShipmentItem>>> GetByShipmentIdAsync(PagedRequest request, Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ShipmentItem>>> GetByOrderItemIdAsync(PagedRequest request, Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<ShipmentItem>> CreateAsync(ShipmentItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ShipmentItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    
    // Shipment item management operations
    Task<Result<bool>> ExistsAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetItemCountByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetTotalQuantityByShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ShipmentItem>>> GetByOrderIdAsync(PagedRequest request, Guid orderId, CancellationToken cancellationToken = default);
}
