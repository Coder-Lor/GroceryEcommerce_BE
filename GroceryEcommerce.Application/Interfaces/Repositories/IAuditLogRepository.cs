using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Auth;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IAuditLogRepository
{
    // Basic CRUD operations
    Task<Result<AuditLog?>> GetByIdAsync(Guid auditId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<AuditLog>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<AuditLog>> CreateAsync(AuditLog auditLog, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid auditId, CancellationToken cancellationToken = default);
    
    // Audit log query operations
    Task<Result<List<AuditLog>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLog>>> GetByEntityAsync(string entity, Guid entityId, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLog>>> GetByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLog>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLog>>> GetByUserAndDateRangeAsync(Guid userId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<AuditLog>>> GetRecentLogsAsync(int count = 100, CancellationToken cancellationToken = default);
    
    // Audit log statistics
    Task<Result<int>> GetLogCountByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetLogCountByActionAsync(string action, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<string, int>>> GetActionStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
