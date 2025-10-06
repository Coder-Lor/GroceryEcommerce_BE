using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IOrderShipmentRepository
{
    // Basic CRUD operations
    Task<Result<OrderShipment?>> GetByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment?>> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment?>> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderShipment>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment>> CreateAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    
    // Shipment management operations
    Task<Result<bool>> ExistsAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShipmentStatusAsync(Guid shipmentId, short status, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateShipmentNumberAsync(CancellationToken cancellationToken = default);
}
