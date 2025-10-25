using GroceryEcommerce.DatabaseSpecific;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IUnitOfWorkService : IDisposable
{
    bool HasActiveTransaction { get; }
    DataAccessAdapter GetAdapter();
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct = default);
}