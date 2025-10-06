using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IOrderPaymentRepository
{
    // Basic CRUD operations
    Task<Result<OrderPayment?>> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderPayment?>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderPayment>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderPayment>> CreateAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid paymentId, CancellationToken cancellationToken = default);
    
    // Payment management operations
    Task<Result<bool>> ExistsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetByPaymentMethodAsync(short paymentMethod, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalPaidByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePaymentStatusAsync(Guid paymentId, short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetFailedPaymentsAsync(CancellationToken cancellationToken = default);
}
