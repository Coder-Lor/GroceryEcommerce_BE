using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IPurchaseOrderRepository : IPagedRepository<PurchaseOrder>
{
    // Basic CRUD operations
    Task<Result<PurchaseOrder?>> GetByIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder?>> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<bool>> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    
    // Purchase order management operations
    Task<Result<bool>> ExistsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStatusAsync(Guid purchaseOrderId, short status, CancellationToken cancellationToken = default);
    
    // Paged queries
    Task<Result<PagedResult<PurchaseOrder>>> GetByStatusAsync(short status, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrder>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrder>>> GetPendingOrdersAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default);
}
