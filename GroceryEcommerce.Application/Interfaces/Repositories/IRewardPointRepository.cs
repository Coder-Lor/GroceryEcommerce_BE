using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Marketing;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IRewardPointRepository
{
    // Basic CRUD operations
    Task<Result<RewardPoint?>> GetByIdAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<List<RewardPoint>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<RewardPoint>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<RewardPoint>> CreateAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    
    // Reward point management operations
    Task<Result<bool>> ExistsAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalPointsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetAvailablePointsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<RewardPoint>>> GetPointsByTypeAsync(Guid userId, short pointType, CancellationToken cancellationToken = default);
    Task<Result<List<RewardPoint>>> GetExpiringPointsAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddPointsAsync(Guid userId, decimal points, short pointType, string description, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeductPointsAsync(Guid userId, decimal points, string description, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExpirePointsAsync(Guid userId, decimal points, CancellationToken cancellationToken = default);
}
