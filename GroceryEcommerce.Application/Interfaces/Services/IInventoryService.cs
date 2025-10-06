using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IInventoryService
{
    // Warehouse services
    Task<Result<List<WarehouseDto>>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    Task<Result<WarehouseDto?>> GetWarehouseByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<WarehouseDto?>> GetWarehouseByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<WarehouseDto>> CreateWarehouseAsync(CreateWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateWarehouseAsync(Guid warehouseId, UpdateWarehouseRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<WarehouseDto>>> GetActiveWarehousesAsync(CancellationToken cancellationToken = default);

    // Supplier services
    Task<Result<List<SupplierDto>>> GetSuppliersAsync(CancellationToken cancellationToken = default);
    Task<Result<SupplierDto?>> GetSupplierByIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<SupplierDto?>> GetSupplierByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<SupplierDto>>> GetSuppliersByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<SupplierDto>> CreateSupplierAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateSupplierAsync(Guid supplierId, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteSupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<List<SupplierDto>>> GetActiveSuppliersAsync(CancellationToken cancellationToken = default);

    // Purchase Order services
    Task<Result<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<PurchaseOrderDto>>> GetPurchaseOrdersBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
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
    Task<Result<List<StockMovementDto>>> GetStockMovementsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovementDto>>> GetStockMovementsByTypeAsync(short movementType, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovementDto>>> GetStockMovementsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<StockMovementDto?>> GetStockMovementByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<StockMovementDto>> CreateStockMovementAsync(CreateStockMovementRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStockMovementAsync(Guid movementId, CreateStockMovementRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteStockMovementAsync(Guid movementId, CancellationToken cancellationToken = default);

    // Inventory calculations
    Task<Result<int>> GetCurrentStockAsync(Guid productId, Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalStockValueAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);
    Task<Result<List<LowStockAlertDto>>> GetLowStockProductsAsync(int threshold = 10, CancellationToken cancellationToken = default);
    Task<Result<List<StockSummaryDto>>> GetOutOfStockProductsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetStockMovementsSummaryAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> AdjustStockAsync(StockAdjustmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<StockSummaryDto>> GetStockSummaryAsync(Guid productId, Guid? warehouseId = null, CancellationToken cancellationToken = default);
}
