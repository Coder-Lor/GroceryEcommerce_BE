using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GroceryEcommerce.Application.Common;

public class PagedRequest
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private int _pageSize = DefaultPageSize;

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    [StringLength(200, ErrorMessage = "Search term cannot exceed 200 characters")]
    public string? Search { get; set; }

    [StringLength(50, ErrorMessage = "Sort field name cannot exceed 50 characters")]
    public string? SortBy { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    
    public List<FilterCriteria> Filters { get; set; } = new();

    // Thêm metadata cho validation
    [JsonIgnore]
    public string? EntityType { get; set; }

    [JsonIgnore]
    public IReadOnlyList<SearchableField>? AvailableFields { get; set; }

    // Helper methods
    public void AddFilter(string fieldName, object value, FilterOperator op = FilterOperator.Equals)
    {
        Filters.Add(new FilterCriteria(fieldName, value, op));
    }

    public void AddFilter(string fieldName, object minValue, object maxValue)
    {
        Filters.Add(new FilterCriteria(fieldName, minValue, FilterOperator.GreaterThanOrEqual));
        Filters.Add(new FilterCriteria(fieldName, maxValue, FilterOperator.LessThanOrEqual));
    }

    public bool HasFilters => Filters.Count > 0;
    
    public bool HasSearch => !string.IsNullOrWhiteSpace(Search);
    
    public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);

    // Generate cache key efficiently
    public string GenerateCacheKey(string entityName)
    {
        var keyParts = new List<string>
        {
            entityName,
            Page.ToString(),
            PageSize.ToString()
        };

        if (HasSearch)
            keyParts.Add($"S:{Search}");

        if (HasSorting)
            keyParts.Add($"SO:{SortBy}:{SortDirection}");

        if (HasFilters)
        {
            var filterKey = string.Join("|", Filters.OrderBy(f => f.FieldName)
                .Select(f => $"{f.FieldName}:{f.Operator}:{f.Value}"));
            keyParts.Add($"F:{filterKey}");
        }

        return string.Join("_", keyParts);
    }
    
    public ValidationResult? Validate()
    {
        var errors = new List<string>();

        if (Page <= 0)
            errors.Add("Page must be greater than 0");

        if (PageSize <= 0 || PageSize > MaxPageSize)
            errors.Add($"PageSize must be between 1 and {MaxPageSize}");

        if (!string.IsNullOrWhiteSpace(Search) && Search.Length > 200)
            errors.Add("Search term cannot exceed 200 characters");

        if (!string.IsNullOrWhiteSpace(SortBy) && AvailableFields != null)
        {
            var sortableField = AvailableFields.FirstOrDefault(f => 
                f.FieldName.Equals(SortBy, StringComparison.OrdinalIgnoreCase));
            
            if (sortableField == null)
                errors.Add($"Field '{SortBy}' is not sortable");
            else if (!sortableField.IsSortable)
                errors.Add($"Field '{SortBy}' is not sortable");
        }


        if (AvailableFields != null)
        {
            foreach (var filter in Filters)
            {
                var searchableField = AvailableFields.FirstOrDefault(f => 
                    f.FieldName.Equals(filter.FieldName, StringComparison.OrdinalIgnoreCase));
                
                if (searchableField == null)
                    errors.Add($"Field '{filter.FieldName}' is not filterable");
                else if (!searchableField.IsFilterable)
                    errors.Add($"Field '{filter.FieldName}' is not filterable");
            }
        }

        return errors.Count == 0 
            ? ValidationResult.Success 
            : new ValidationResult(string.Join("; ", errors));
    }
}

public enum SortDirection
{
    Ascending,
    Descending
}

public static class PagedRequestExtensions
{
    public static PagedRequest WithSearch(this PagedRequest request, string searchTerm)
    {
        request.Search = searchTerm;
        return request;
    }

    public static PagedRequest WithSorting(this PagedRequest request, string sortBy, SortDirection direction = SortDirection.Ascending)
    {
        request.SortBy = sortBy;
        request.SortDirection = direction;
        return request;
    }

    public static PagedRequest WithPaging(this PagedRequest request, int page, int pageSize)
    {
        request.Page = page;
        request.PageSize = pageSize;
        return request;
    }

    public static PagedRequest EnsureSorting(this PagedRequest request, string defaultSortBy, SortDirection defaultDirection = SortDirection.Ascending)
    {
        if (!request.HasSorting)
            request.WithSorting(defaultSortBy, defaultDirection);
        return request;
    }

    public static PagedRequest WithRangeFilter(this PagedRequest request, string fieldName, object minValue, object maxValue)
    {
        request.AddFilter(fieldName, minValue, FilterOperator.GreaterThanOrEqual);
        request.AddFilter(fieldName, maxValue, FilterOperator.LessThanOrEqual);
        return request;
    }

    public static PagedRequest WithFilter(this PagedRequest request, string fieldName, object value, FilterOperator op = FilterOperator.Equals)
    {
        request.AddFilter(fieldName, value, op);
        return request;
    }

    // Overloads to improve clarity with Guid and bool values
    public static PagedRequest WithFilter(this PagedRequest request, string fieldName, Guid value)
    {
        request.AddFilter(fieldName, value, FilterOperator.Equals);
        return request;
    }

    public static PagedRequest WithFilter(this PagedRequest request, string fieldName, bool value)
    {
        request.AddFilter(fieldName, value, FilterOperator.Equals);
        return request;
    }

    public static PagedRequest WithFilter(this PagedRequest request, string fieldName, object value)
    {
        request.AddFilter(fieldName, value, FilterOperator.Equals);
        return request;
    }
}