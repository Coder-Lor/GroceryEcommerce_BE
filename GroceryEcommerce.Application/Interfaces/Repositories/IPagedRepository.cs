using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IPagedRepository<TEntity>
{
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    IReadOnlyList<SearchableField> GetSearchableFields();
}