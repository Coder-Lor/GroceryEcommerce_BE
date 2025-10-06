using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Reviews;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IReviewsRepository
{
    // Product Review operations
    Task<Result<PagedResult<ProductReview>>> GetProductReviewsAsync(Guid productId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReview>>> GetProductReviewsByRatingAsync(Guid productId, int rating, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReview>>> GetProductReviewsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ProductReview?>> GetProductReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ProductReview?>> GetProductReviewByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductReview>> CreateProductReviewAsync(ProductReview review, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductReviewAsync(ProductReview review, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ApproveProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RejectProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReview>>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ProductReview>>> GetApprovedReviewsAsync(Guid productId, CancellationToken cancellationToken = default);

    // Review Image operations
    Task<Result<List<ReviewImage>>> GetReviewImagesAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ReviewImage?>> GetReviewImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<ReviewImage>> CreateReviewImageAsync(ReviewImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewImageAsync(ReviewImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewImageAsync(Guid imageId, CancellationToken cancellationToken = default);

    // Review Vote operations
    Task<Result<List<ReviewVote>>> GetReviewVotesAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVote?>> GetReviewVoteByUserAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVote>> CreateReviewVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewVoteAsync(ReviewVote vote, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewVoteAsync(Guid voteId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetReviewVoteCountAsync(Guid reviewId, bool isHelpful, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasUserVotedAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);

    // Review Report operations
    Task<Result<List<ReviewReport>>> GetReviewReportsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetReviewReportsByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetReviewReportsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewReport?>> GetReviewReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<ReviewReport>> CreateReviewReportAsync(ReviewReport report, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewReportAsync(ReviewReport report, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ResolveReviewReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReport>>> GetPendingReportsAsync(CancellationToken cancellationToken = default);

    // Review analytics
    Task<Result<decimal>> GetProductAverageRatingAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<int, int>>> GetProductRatingDistributionAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductReviewCountAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetTopRatedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetMostReviewedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetReviewStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> CanUserReviewProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
}
