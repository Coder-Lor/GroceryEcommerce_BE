using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Reviews;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IReviewsService
{
    // Product Review services
    Task<Result<PagedResult<ProductReviewDto>>> GetProductReviewsAsync(Guid productId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReviewDto>>> GetProductReviewsByRatingAsync(Guid productId, int rating, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReviewDto>>> GetProductReviewsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ProductReviewDto?>> GetProductReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ProductReviewDto?>> GetProductReviewByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductReviewDto>> CreateProductReviewAsync(CreateProductReviewRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductReviewAsync(Guid reviewId, UpdateProductReviewRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ApproveProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RejectProductReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductReviewDto>>> GetPendingReviewsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ProductReviewDto>>> GetApprovedReviewsAsync(Guid productId, CancellationToken cancellationToken = default);

    // Review Image services
    Task<Result<List<ReviewImageDto>>> GetReviewImagesAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ReviewImageDto?>> GetReviewImageByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<ReviewImageDto>> CreateReviewImageAsync(CreateReviewImageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewImageAsync(Guid imageId, UpdateReviewImageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewImageAsync(Guid imageId, CancellationToken cancellationToken = default);

    // Review Vote services
    Task<Result<List<ReviewVoteDto>>> GetReviewVotesAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVoteDto?>> GetReviewVoteByUserAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVoteDto>> CreateReviewVoteAsync(CreateReviewVoteRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewVoteAsync(Guid voteId, CreateReviewVoteRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewVoteAsync(Guid voteId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetReviewVoteCountAsync(Guid reviewId, bool isHelpful, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasUserVotedAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);

    // Review Report services
    Task<Result<List<ReviewReportDto>>> GetReviewReportsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReportDto>>> GetReviewReportsByReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReportDto>>> GetReviewReportsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewReportDto?>> GetReviewReportByIdAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<ReviewReportDto>> CreateReviewReportAsync(CreateReviewReportRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateReviewReportAsync(Guid reportId, UpdateReviewReportRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteReviewReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ResolveReviewReportAsync(Guid reportId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewReportDto>>> GetPendingReportsAsync(CancellationToken cancellationToken = default);

    // Review analytics
    Task<Result<decimal>> GetProductAverageRatingAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<Dictionary<int, int>>> GetProductRatingDistributionAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetProductReviewCountAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetTopRatedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetMostReviewedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetReviewStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<bool>> CanUserReviewProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductReviewSummaryDto>> GetProductReviewSummaryAsync(Guid productId, CancellationToken cancellationToken = default);
}
