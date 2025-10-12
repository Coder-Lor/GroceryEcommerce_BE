using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Reviews;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Reviews;

public interface IReviewImageRepository
{
    // Basic CRUD operations
    Task<Result<ReviewImage?>> GetByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewImage>>> GetByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ReviewImage>> CreateAsync(ReviewImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ReviewImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
    
    // Review image management operations
    Task<Result<bool>> ExistsAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetImageCountByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewImage>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
