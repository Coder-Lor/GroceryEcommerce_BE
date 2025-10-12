using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Marketing;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Marketing;

public interface ICouponUsageRepository
{
    // Basic CRUD operations
    Task<Result<CouponUsage?>> GetByIdAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsage>>> GetByCouponIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsage>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsage>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<CouponUsage>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<CouponUsage>> CreateAsync(CouponUsage usage, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(CouponUsage usage, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid usageId, CancellationToken cancellationToken = default);
    
    // Coupon usage management operations
    Task<Result<bool>> ExistsAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUsageCountByCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUsageCountByUserAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasUserUsedCouponAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalDiscountByCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalDiscountByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsage>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
