
using GroceryEcommerce.Application.Common;

namespace GroceryEcommerce.API.Contracts.Requests;

public interface IPagedRepository<TEntity>
{
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    IReadOnlyList<SearchableField> GetSearchableFields();
}