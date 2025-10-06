using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Application.Models.Cart;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ISalesService
{
    // Order services
    Task<Result<PagedResult<OrderDto>>> GetOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<OrderDto>>> GetOrdersByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<OrderDetailDto?>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderDetailDto?>> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<List<OrderDto>>> GetOrdersByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderDto>>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderAsync(Guid orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, short status, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderPaymentStatusAsync(Guid orderId, short paymentStatus, CancellationToken cancellationToken = default);
    Task<Result<bool>> CancelOrderAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);

    // Order Item services
    Task<Result<List<OrderItemDto>>> GetOrderItemsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderItemDto?>> GetOrderItemByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default);
    Task<Result<OrderItemDto>> CreateOrderItemAsync(CreateOrderItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderItemAsync(Guid orderItemId, UpdateOrderItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default);

    // Order Status History services
    Task<Result<List<OrderStatusHistoryDto>>> GetOrderStatusHistoryAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderStatusHistoryDto>> CreateOrderStatusHistoryAsync(CreateOrderStatusHistoryRequest request, CancellationToken cancellationToken = default);

    // Order Payment services
    Task<Result<List<OrderPaymentDto>>> GetOrderPaymentsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderPaymentDto?>> GetOrderPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<OrderPaymentDto>> CreateOrderPaymentAsync(CreateOrderPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderPaymentAsync(Guid paymentId, UpdateOrderPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ProcessPaymentAsync(Guid orderId, ProcessPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> RefundPaymentAsync(Guid orderId, decimal amount, string reason, CancellationToken cancellationToken = default);

    // Order Shipment services
    Task<Result<List<OrderShipmentDto>>> GetOrderShipmentsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderShipmentDto?>> GetOrderShipmentByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Result<OrderShipmentDto>> CreateOrderShipmentAsync(CreateOrderShipmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderShipmentAsync(Guid shipmentId, UpdateOrderShipmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ShipOrderAsync(Guid orderId, CreateOrderShipmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkAsDeliveredAsync(Guid orderId, CancellationToken cancellationToken = default);

    // Shipment Carrier services
    Task<Result<List<ShipmentCarrierDto>>> GetShipmentCarriersAsync(CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrierDto?>> GetShipmentCarrierByIdAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrierDto>> CreateShipmentCarrierAsync(CreateShipmentCarrierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateShipmentCarrierAsync(Guid carrierId, UpdateShipmentCarrierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteShipmentCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default);

    // Order Refund services
    Task<Result<List<OrderRefundDto>>> GetOrderRefundsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderRefundDto?>> GetOrderRefundByIdAsync(Guid refundId, CancellationToken cancellationToken = default);
    Task<Result<OrderRefundDto>> CreateOrderRefundAsync(CreateOrderRefundRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateOrderRefundAsync(Guid refundId, UpdateOrderRefundRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ProcessRefundAsync(Guid orderId, decimal amount, string reason, CancellationToken cancellationToken = default);

    // Checkout services
    Task<Result<CheckoutDto>> GetCheckoutInfoAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> ProcessCheckoutAsync(ProcessCheckoutRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCheckoutAsync(ValidateCheckoutRequest request, CancellationToken cancellationToken = default);
    Task<Result<decimal>> CalculateShippingCostAsync(CalculateShippingRequest request, CancellationToken cancellationToken = default);
    Task<Result<decimal>> CalculateTaxAsync(CalculateTaxRequest request, CancellationToken cancellationToken = default);

    // Sales analytics
    Task<Result<SalesAnalyticsDto>> GetSalesAnalyticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalSalesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<int>> GetOrderCountByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetAverageOrderValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<TopSellingProductDto>>> GetTopSellingProductsAsync(int limit = 10, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default);
    Task<Result<List<SalesByStatusDto>>> GetSalesByStatusAsync(CancellationToken cancellationToken = default);
}
