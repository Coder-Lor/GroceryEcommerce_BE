namespace GroceryEcommerce.Application.Models;

// DTOs for Service Interfaces that were referenced but not created

public class InventoryServiceDto
{
    // This will be implemented when creating the actual service
}

public class MarketingServiceDto
{
    // This will be implemented when creating the actual service
}

public class ReviewsServiceDto
{
    // This will be implemented when creating the actual service
}

public class SystemServiceDto
{
    // This will be implemented when creating the actual service
}

// Additional DTOs that might be needed
public class ApiResponseDto<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class PaginationDto
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}

public class FilterDto
{
    public string? SearchTerm { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
    public string? SortBy { get; set; }
    public string? SortDirection { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class BulkOperationDto
{
    public List<Guid> Ids { get; set; } = new();
    public string Operation { get; set; } = string.Empty;
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class BulkOperationResultDto
{
    public int TotalProcessed { get; set; }
    public int Successful { get; set; }
    public int Failed { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<Guid> SuccessfulIds { get; set; } = new();
    public List<Guid> FailedIds { get; set; } = new();
}
