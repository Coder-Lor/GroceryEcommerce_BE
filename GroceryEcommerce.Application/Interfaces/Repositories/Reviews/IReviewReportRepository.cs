using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Reviews;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Reviews;

public interface IReviewReportRepository
{
    // Basic CRUD operations
    Task<Result<ReviewReport?>> GetByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ReviewReport>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ReviewReport>> CreateAsync(ReviewReport report, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ReviewReport report, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid reportId, CancellationToken cancellationToken = default);
    
    // Review report management operations
    Task<Result<bool>> ExistsAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetUnprocessedReportsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetProcessedReportsAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> ProcessReportAsync(Guid reportId, Guid processedBy, CancellationToken cancellationToken = default);
    Task<Result<int>> GetReportCountByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUnprocessedCountAsync(CancellationToken cancellationToken = default);
}
