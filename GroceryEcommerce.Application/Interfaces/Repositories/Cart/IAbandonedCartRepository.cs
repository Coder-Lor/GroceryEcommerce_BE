using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Cart;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Cart;

public interface IAbandonedCartRepository
{
    // Basic CRUD operations
    Task<Result<AbandonedCart?>> GetByIdAsync(Guid abandonedId, CancellationToken cancellationToken = default);
    Task<Result<List<AbandonedCart>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<AbandonedCart>> CreateAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(AbandonedCart abandonedCart, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid abandonedId, CancellationToken cancellationToken = default);
    
    // Abandoned cart management operations
    Task<Result<List<AbandonedCart>>> GetUnnotifiedCartsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<AbandonedCart>>> GetUnnotifiedCartsByTimeRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkAsNotifiedAsync(Guid abandonedId, CancellationToken cancellationToken = default);
    Task<Result<bool>> MarkAsNotifiedAsync(List<Guid> abandonedIds, CancellationToken cancellationToken = default);
    Task<Result<List<AbandonedCart>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<int>> GetAbandonedCartCountAsync(CancellationToken cancellationToken = default);
    Task<Result<int>> GetUnnotifiedCartCountAsync(CancellationToken cancellationToken = default);
}
