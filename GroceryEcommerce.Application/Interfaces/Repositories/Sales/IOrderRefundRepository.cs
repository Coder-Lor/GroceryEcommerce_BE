using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderRefundRepository
{
    // Basic CRUD operations
    Task<Result<OrderRefund?>> GetByIdAsync(Guid refundId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderRefund>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderRefund>>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderRefund>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderRefund>> CreateAsync(OrderRefund refund, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderRefund refund, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid refundId, CancellationToken cancellationToken = default);
    
    // Refund management operations
    Task<Result<bool>> ExistsAsync(Guid refundId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderRefund>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderRefund>>> GetPendingRefundsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<OrderRefund>>> GetProcessedRefundsAsync(CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalRefundedByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ProcessRefundAsync(Guid refundId, Guid processedBy, CancellationToken cancellationToken = default);
    Task<Result<int>> GetPendingRefundCountAsync(CancellationToken cancellationToken = default);
}
