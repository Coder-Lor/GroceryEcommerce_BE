using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IStockMovementRepository
{
    // Basic CRUD operations
    Task<Result<StockMovement?>> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<StockMovement>> CreateAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid movementId, CancellationToken cancellationToken = default);
    
    // Stock movement management operations
    Task<Result<bool>> ExistsAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetByMovementTypeAsync(short movementType, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetByProductAndWarehouseAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetCurrentStockAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<List<StockMovement>>> GetRecentMovementsAsync(int count = 100, CancellationToken cancellationToken = default);
}
