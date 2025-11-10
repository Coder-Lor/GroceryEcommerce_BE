using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IStockMovementRepository
{
    // Basic CRUD operations
    Task<Result<StockMovement?>> GetByIdAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetByProductIdAsync(Guid productId, PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<StockMovement>> CreateAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(StockMovement movement, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid movementId, CancellationToken cancellationToken = default);
    
    // Stock movement management operations
    Task<Result<bool>> ExistsAsync(Guid movementId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetByDateRangeAsync(PagedRequest pagedRequest, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetByMovementTypeAsync(PagedRequest pagedRequest, short movementType, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetCurrentStockAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StockMovement>>> GetRecentMovementsAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default);
}
