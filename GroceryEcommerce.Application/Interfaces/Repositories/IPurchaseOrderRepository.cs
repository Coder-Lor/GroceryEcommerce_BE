using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IPurchaseOrderRepository
{
    // Basic CRUD operations
    Task<Result<PurchaseOrder?>> GetByIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder?>> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetBySupplierIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<PurchaseOrder>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder>> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    
    // Purchase order management operations
    Task<Result<bool>> ExistsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStatusAsync(Guid purchaseOrderId, short status, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetPendingOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalValueBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
