namespace GroceryEcommerce.Application.Common;

public class PagedResult<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
{
    public IReadOnlyList<T> Items { get; set; } = items;
    public int TotalCount { get; init; } = totalCount;
    public int Page { get; init; } = page;
    public int PageSize { get; init; } = pageSize;
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}