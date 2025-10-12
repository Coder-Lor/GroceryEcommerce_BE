using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.System;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Systems;

public interface ICurrencyRepository
{
    // Basic CRUD operations
    Task<Result<Currency?>> GetByIdAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<Currency?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<Currency>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Currency>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Currency>> CreateAsync(Currency currency, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Currency currency, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid currencyId, CancellationToken cancellationToken = default);
    
    // Currency management operations
    Task<Result<bool>> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<List<Currency>>> GetActiveCurrenciesAsync(CancellationToken cancellationToken = default);
    Task<Result<Currency?>> GetDefaultCurrencyAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> SetDefaultCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsDefaultCurrencyAsync(Guid currencyId, CancellationToken cancellationToken = default);
}
