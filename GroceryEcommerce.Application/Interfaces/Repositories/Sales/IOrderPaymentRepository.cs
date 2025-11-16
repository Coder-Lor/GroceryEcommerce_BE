using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderPaymentRepository : IPagedRepository<OrderPayment>
{
    // Basic CRUD operations
    Task<Result<OrderPayment?>> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderPayment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<OrderPayment?>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderPayment payment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid paymentId, CancellationToken cancellationToken = default);
    
    // Payment management operations
    Task<Result<bool>> ExistsAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderPayment>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderPayment>>> GetByPaymentMethodAsync(short paymentMethod, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderPayment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalPaidByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePaymentStatusAsync(Guid paymentId, short status, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderPayment>>> GetFailedPaymentsAsync(PagedRequest request, CancellationToken cancellationToken = default);
}
