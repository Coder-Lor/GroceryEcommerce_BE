namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IUnitOfWork : IDisposable
{
    bool HasActiveTransaction { get; }
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}