using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Inventory;

public interface IWarehouseRepository
{
    // Basic CRUD operations
    Task<Result<Warehouse?>> GetByIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<Warehouse?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<Warehouse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Warehouse>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Warehouse>> CreateAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid warehouseId, CancellationToken cancellationToken = default);

    // Warehouse management operations
    Task<Result<bool>> ExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Warehouse>>> GetActiveWarehousesAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductCountByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsWarehouseInUseAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}