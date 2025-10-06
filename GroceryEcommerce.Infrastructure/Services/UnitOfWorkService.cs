using System.Data;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GroceryEcommerce.Infrastructure.Services;

public class UnitOfWorkService(DataAccessAdapter adapter) : IUnitOfWorkService
{
    private int _txDepth = 0;
    public bool HasActiveTransaction => _txDepth > 0;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_txDepth == 0)
        {
            await adapter.StartTransactionAsync(IsolationLevel.ReadCommitted, "UnitOfWork", ct);
        }
        _txDepth++;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_txDepth <= 0) return;
        _txDepth--;
        if (_txDepth == 0)
        {
            await adapter.CommitAsync(ct);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => Task.FromResult(0); 

    public Task RollbackAsync(CancellationToken ct = default)
    {
        if (_txDepth > 0)
        {
            adapter.Rollback();
            _txDepth = 0;
        }
        return Task.CompletedTask;
    }
    
    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> work, CancellationToken ct = default)
    {
        await BeginTransactionAsync(ct);
        try
        {
            await work(ct);
            await CommitAsync(ct);
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
    }

    public void Dispose() => adapter.Dispose();

    public ValueTask DisposeAsync()
    {
        adapter.Dispose();
        return ValueTask.CompletedTask;
    }
}