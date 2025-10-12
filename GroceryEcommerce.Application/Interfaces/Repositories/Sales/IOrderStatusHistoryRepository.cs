using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderStatusHistoryRepository
{
    // Basic CRUD operations
    Task<Result<OrderStatusHistory?>> GetByIdAsync(Guid historyId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderStatusHistory>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderStatusHistory>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderStatusHistory>> CreateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid historyId, CancellationToken cancellationToken = default);
    
    // Status history management operations
    Task<Result<bool>> ExistsAsync(Guid historyId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderStatusHistory>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<OrderStatusHistory>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderStatusHistory>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<OrderStatusHistory?>> GetLatestByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddStatusChangeAsync(Guid orderId, short fromStatus, short toStatus, string? comment, Guid createdBy, CancellationToken cancellationToken = default);
}
