using GroceryEcommerce.Application.Common;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Common;

public interface IPagedRepository<TEntity>
{
    Task<Result<PagedResult<TEntity>>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    IReadOnlyList<SearchableField> GetSearchableFields();
    string? GetDefaultSortField();
    IReadOnlyList<FieldMapping> GetFieldMappings();
}