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

    // Stock Movement services
    Task<Result<PagedResult<StockMovementDto>>> GetStockMovementsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovementDto>>> GetStockMovementsByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovementDto>>> GetStockMovementsByTypeAsync(short movementType, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovementDto>>> GetStockMovementsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<StockMovementDto?>> GetStockMovementByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<StockMovementDto>> CreateStockMovementAsync(CreateStockMovementRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockMovementAsync(Guid movementId, CreateStockMovementRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteStockMovementAsync(Guid movementId, CancellationToken cancellationToken = default);

    // Inventory calculations
    Task<Result<int>> GetCurrentStockAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
    Task<Result<List<LowStockAlertDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<Result<List<StockSummaryDto>>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetStockMovementsSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> AdjustStockAsync(StockAdjustmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<StockSummaryDto>> GetStockSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
}
