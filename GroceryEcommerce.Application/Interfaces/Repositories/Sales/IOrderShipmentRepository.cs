using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderShipmentRepository : IPagedRepository<OrderShipment>
{
    // Basic CRUD operations
    Task<Result<OrderShipment?>> GetByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderShipment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment?>> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment?>> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    
    // Shipment management operations
    Task<Result<bool>> ExistsAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderShipment>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderShipment>>> GetByCarrierAsync(Guid carrierId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderShipment>>> GetByWarehouseAsync(Guid warehouseId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderShipment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShipmentStatusAsync(Guid shipmentId, short status, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateShipmentNumberAsync(CancellationToken cancellationToken = default);
}
