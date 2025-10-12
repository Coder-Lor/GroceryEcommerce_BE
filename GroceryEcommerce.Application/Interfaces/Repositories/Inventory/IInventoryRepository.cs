using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IInventoryRepository
{
    // Warehouse operations
    Task<Result<List<Warehouse>>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<Result<Warehouse?>> GetWarehouseByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<Warehouse?>> GetWarehouseByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<Warehouse>> CreateWarehouseAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateWarehouseAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<Warehouse>>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default);

    // Supplier operations
    Task<Result<List<Supplier>>> GetSuppliersAsync(CancellationToken cancellationToken = default);
    Task<Result<Supplier?>> GetSupplierByIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<Supplier?>> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<Supplier>>> GetSuppliersByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<Supplier>> CreateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSupplierAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteSupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<List<Supplier>>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default);

    // Purchase Order operations
    Task<Result<PagedResult<PurchaseOrder>>> GetPurchaseOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetPurchaseOrdersBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetPurchaseOrdersByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrder>>> GetPurchaseOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder?>> GetPurchaseOrderByIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder?>> GetPurchaseOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrder>> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePurchaseOrderStatusAsync(Guid purchaseOrderId, short status, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePurchaseOrderAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<string>> GeneratePurchaseOrderNumberAsync(CancellationToken cancellationToken = default);

    // Purchase Order Item operations
    Task<Result<List<PurchaseOrderItem>>> GetPurchaseOrderItemsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderItem?>> GetPurchaseOrderItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderItem>> CreatePurchaseOrderItemAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdatePurchaseOrderItemAsync(PurchaseOrderItem item, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeletePurchaseOrderItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    // Stock Movement operations
    Task<Result<PagedResult<StockMovement>>> GetStockMovementsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetStockMovementsByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetStockMovementsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetStockMovementsByTypeAsync(short movementType, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetStockMovementsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<StockMovement?>> GetStockMovementByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<StockMovement>> CreateStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockMovementAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteStockMovementAsync(Guid movementId, CancellationToken cancellationToken = default);

    // Inventory calculations
    Task<Result<int>> GetCurrentStockAsync(Guid productId, Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalStockValueAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetStockMovementsSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> AdjustStockAsync(Guid productId, int quantity, short movementType, string reason, Guid? warehouseId = null, CancellationToken cancellationToken = default);
}
