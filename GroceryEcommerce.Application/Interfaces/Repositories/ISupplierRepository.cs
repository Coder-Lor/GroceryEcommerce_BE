using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Inventory;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface ISupplierRepository
{
    // Basic CRUD operations
    Task<Result<Supplier?>> GetByIdAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<Supplier?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<Supplier?>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Result<List<Supplier>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Supplier>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Supplier>> CreateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Supplier supplier, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid supplierId, CancellationToken cancellationToken = default);
    
    // Supplier management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<List<Supplier>>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<List<Supplier>>> SearchByContactAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsSupplierInUseAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetPurchaseOrderCountBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
