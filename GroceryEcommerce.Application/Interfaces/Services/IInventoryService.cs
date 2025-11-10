using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IInventoryService
{
    // Purchase Order services
    Task<Result<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderDto>>> GetPurchaseOrdersByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderDto>>> GetPurchaseOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto?>> GetPurchaseOrderByIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto?>> GetPurchaseOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePurchaseOrderAsync(Guid purchaseOrderId, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePurchaseOrderStatusAsync(Guid purchaseOrderId, short status, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<string>> GeneratePurchaseOrderNumberAsync(CancellationToken cancellationToken = default);

}
