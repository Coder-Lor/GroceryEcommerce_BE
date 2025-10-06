namespace GroceryEcommerce.Application.Models.Reviews;

public class ProductReviewDto
{
    public Guid ProductReviewId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public List<ReviewImageDto> Images { get; set; } = new();
    public ReviewVoteSummaryDto VoteSummary { get; set; } = new();
    public bool CanUserVote { get; set; }
    public bool CanUserEdit { get; set; }
}

public class CreateProductReviewRequest
{
    public Guid ProductId { get; set; }
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public List<CreateReviewImageRequest>? Images { get; set; }
}

public class UpdateProductReviewRequest
{
    public int Rating { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class ReviewImageDto
{
    public Guid ReviewImageId { get; set; }
    public Guid ProductReviewId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateReviewImageRequest
{
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public int DisplayOrder { get; set; }
}

public class ReviewVoteDto
{
    public Guid ReviewVoteId { get; set; }
    public Guid ProductReviewId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsHelpful { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ReviewVoteSummaryDto
{
    public int TotalVotes { get; set; }
    public int HelpfulVotes { get; set; }
    public int NotHelpfulVotes { get; set; }
    public decimal HelpfulPercentage { get; set; }
}

public class CreateReviewVoteRequest
{
    public Guid ProductReviewId { get; set; }
    public bool IsHelpful { get; set; }
}

public class ReviewReportDto
{
    public Guid ReviewReportId { get; set; }
    public Guid ProductReviewId { get; set; }
    public string ReviewTitle { get; set; } = string.Empty;
    public Guid ReportedBy { get; set; }
    public string ReportedByName { get; set; } = string.Empty;
    public short ReportReason { get; set; }
    public string ReportReasonName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public Guid? ResolvedBy { get; set; }
    public string? ResolvedByName { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class CreateReviewReportRequest
{
    public Guid ProductReviewId { get; set; }
    public short ReportReason { get; set; }
    public string? Description { get; set; }
}

public class UpdateReviewReportRequest
{
    public short Status { get; set; }
    public string? ResolutionNotes { get; set; }
}

public class ProductReviewSummaryDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public Dictionary<int, int> RatingDistribution { get; set; } = new();
    public List<ProductReviewDto> RecentReviews { get; set; } = new();
    public List<ProductReviewDto> TopReviews { get; set; } = new();
}
