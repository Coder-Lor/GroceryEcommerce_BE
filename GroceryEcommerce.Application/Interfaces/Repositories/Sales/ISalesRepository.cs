using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface ISalesRepository : IPagedRepository<Order>
{
    // Order operations
    Task<Result<PagedResult<Order>>> GetOrdersByUserIdAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Order?>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<Order?>> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Order>>> GetOrdersByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Order>>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderAsync(Order order, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, short status, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderPaymentStatusAsync(Guid orderId, short paymentStatus, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);

    // Order Item operations
    Task<Result<List<OrderItem>>> GetOrderItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderItem?>> GetOrderItemByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default);

    // Order Status History operations
    Task<Result<List<OrderStatusHistory>>> GetOrderStatusHistoryAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderStatusHistoryAsync(OrderStatusHistory statusHistory, CancellationToken cancellationToken = default);

    // Order Payment operations
    Task<Result<List<OrderPayment>>> GetOrderPaymentsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderPayment?>> GetOrderPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderPaymentAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderPaymentAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);

    // Order Shipment operations
    Task<Result<List<OrderShipment>>> GetOrderShipmentsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderShipment?>> GetOrderShipmentByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderShipmentAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderShipmentAsync(OrderShipment shipment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default);

    // Shipment Item operations
    Task<Result<List<ShipmentItem>>> GetShipmentItemsAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<ShipmentItem?>> GetShipmentItemByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateShipmentItemAsync(ShipmentItem shipmentItem, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShipmentItemAsync(ShipmentItem shipmentItem, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default);

    // Shipment Carrier operations
    Task<Result<List<ShipmentCarrier>>> GetShipmentCarriersAsync(CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrier?>> GetShipmentCarrierByIdAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateShipmentCarrierAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShipmentCarrierAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteShipmentCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default);

    // Order Refund operations
    Task<Result<List<OrderRefund>>> GetOrderRefundsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderRefund?>> GetOrderRefundByIdAsync(Guid refundId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateOrderRefundAsync(OrderRefund refund, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderRefundAsync(OrderRefund refund, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderRefundAsync(Guid refundId, CancellationToken cancellationToken = default);

    // Sales analytics
    Task<Result<decimal>> GetTotalSalesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<int>> GetOrderCountByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetAverageOrderValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetTopSellingProductsAsync(int limit = 10, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetSalesByStatusAsync(CancellationToken cancellationToken = default);
}
