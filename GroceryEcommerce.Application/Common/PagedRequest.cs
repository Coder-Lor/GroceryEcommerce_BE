using System.ComponentModel.DataAnnotations;

namespace GroceryEcommerce.Application.Common;

public class PagedRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? Search { get; set; } = string.Empty;
    public string? SortBy { get; set; }
    public bool IsSortDescending { get; set; } = false;
    public Dictionary<string, object>? Filters { get; set; }

    public void AddFilter(string field, object value)
    {
        Filters ??= new Dictionary<string, object>();
        Filters[field] = value;
    }
}