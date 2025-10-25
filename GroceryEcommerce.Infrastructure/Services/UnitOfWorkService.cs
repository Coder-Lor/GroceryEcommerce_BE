using System.Data;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;

namespace GroceryEcommerce.Infrastructure.Services;

public class UnitOfWorkService : IUnitOfWorkService
{
    private readonly IDataAccessAdapterFactory _adapterFactory;
    private DataAccessAdapter? _transactionAdapter;
    private int _txDepth;
    
    public UnitOfWorkService(IDataAccessAdapterFactory adapterFactory)
    {
        _adapterFactory = adapterFactory;
    }
    
    public bool HasActiveTransaction => _txDepth > 0;
    
    public DataAccessAdapter GetAdapter()
    {
        if (_transactionAdapter == null)
        {
            throw new InvalidOperationException("No active transaction. Call BeginTransactionAsync first.");
        }
        return _transactionAdapter;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_txDepth == 0)
        {
            _transactionAdapter = (DataAccessAdapter)_adapterFactory.CreateAdapter();
            await _transactionAdapter.StartTransactionAsync(IsolationLevel.ReadCommitted, "UnitOfWork", ct);
        }
        _txDepth++;
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_txDepth <= 0) return;
        _txDepth--;
        if (_txDepth == 0)
        {
            await _transactionAdapter!.CommitAsync(ct);
            _transactionAdapter.Dispose();
            _transactionAdapter = null;
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => Task.FromResult(0); 

    public Task RollbackAsync(CancellationToken ct = default)
    {
        if (_txDepth > 0)
        {
            _transactionAdapter?.Rollback();
            _transactionAdapter?.Dispose();
            _transactionAdapter = null;
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

    public void Dispose()
    {
        _transactionAdapter?.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        _transactionAdapter?.Dispose();
        return ValueTask.CompletedTask;
    }
}