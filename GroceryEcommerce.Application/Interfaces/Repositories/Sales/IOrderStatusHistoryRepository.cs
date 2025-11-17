using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Sales;

public interface IOrderStatusHistoryRepository : IPagedRepository<OrderStatusHistory>
{
    // Basic CRUD operations
    Task<Result<OrderStatusHistory?>> GetByIdAsync(Guid historyId, CancellationToken cancellationToken = default);
    Task<Result<List<OrderStatusHistory>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(OrderStatusHistory history, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid historyId, CancellationToken cancellationToken = default);
    
    // Status history management operations
    Task<Result<bool>> ExistsAsync(Guid historyId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderStatusHistory>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderStatusHistory>>> GetByUserAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderStatusHistory>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderStatusHistory?>> GetLatestByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddStatusChangeAsync(Guid orderId, short fromStatus, short toStatus, string? comment, Guid createdBy, CancellationToken cancellationToken = default);
}
