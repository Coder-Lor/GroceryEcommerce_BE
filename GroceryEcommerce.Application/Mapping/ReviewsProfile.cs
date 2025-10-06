using AutoMapper;
using GroceryEcommerce.Application.Models.Reviews;
using GroceryEcommerce.Domain.Entities.Reviews;

namespace GroceryEcommerce.Application.Mapping;

public class ReviewsProfile : Profile
{
    public ReviewsProfile()
    {
        // Product Review mappings
        CreateMap<ProductReview, ProductReviewDto>()
            .ForMember(dest => dest.ProductReviewId, opt => opt.MapFrom(src => src.ReviewId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => (int)src.Rating))
            .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetReviewStatusName(src.Status)))
            .ForMember(dest => dest.ApprovedByName, opt => opt.MapFrom(src => (string?)null)) // Entity doesn't have ApprovedByUser
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ReviewImages))
            .ForMember(dest => dest.VoteSummary, opt => opt.MapFrom(src => new ReviewVoteSummaryDto
            {
                TotalVotes = src.ReviewVotes.Count,
                HelpfulVotes = src.ReviewVotes.Count(v => v.Helpful),
                NotHelpfulVotes = src.ReviewVotes.Count(v => !v.Helpful),
                HelpfulPercentage = src.ReviewVotes.Any() ? (decimal)src.ReviewVotes.Count(v => v.Helpful) / src.ReviewVotes.Count * 100 : 0
            }))
            .ForMember(dest => dest.CanUserVote, opt => opt.Ignore()) // Will be set in service
            .ForMember(dest => dest.CanUserEdit, opt => opt.Ignore()); // Will be set in service

        CreateMap<CreateProductReviewRequest, ProductReview>()
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => (short)src.Rating))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateProductReviewRequest, ProductReview>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => (short)src.Rating))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Comment))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Review Image mappings
        CreateMap<ReviewImage, ReviewImageDto>()
            .ForMember(dest => dest.ReviewImageId, opt => opt.MapFrom(src => src.ImageId))
            .ForMember(dest => dest.ProductReviewId, opt => opt.MapFrom(src => src.ReviewId))
            .ForMember(dest => dest.AltText, opt => opt.MapFrom(src => (string?)null)) // Entity doesn't have AltText
            .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => 0)); // Entity doesn't have DisplayOrder

        CreateMap<CreateReviewImageRequest, ReviewImage>()
            .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateReviewImageRequest, ReviewImage>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl));

        // Review Vote mappings
        CreateMap<ReviewVote, ReviewVoteDto>()
            .ForMember(dest => dest.ReviewVoteId, opt => opt.MapFrom(src => src.VoteId))
            .ForMember(dest => dest.ProductReviewId, opt => opt.MapFrom(src => src.ReviewId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.IsHelpful, opt => opt.MapFrom(src => src.Helpful));

        CreateMap<CreateReviewVoteRequest, ReviewVote>()
            .ForMember(dest => dest.VoteId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ProductReviewId))
            .ForMember(dest => dest.Helpful, opt => opt.MapFrom(src => src.IsHelpful))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Review Report mappings
        CreateMap<ReviewReport, ReviewReportDto>()
            .ForMember(dest => dest.ReviewReportId, opt => opt.MapFrom(src => src.ReportId))
            .ForMember(dest => dest.ProductReviewId, opt => opt.MapFrom(src => src.ReviewId))
            .ForMember(dest => dest.ReviewTitle, opt => opt.MapFrom(src => src.ProductReview.Title))
            .ForMember(dest => dest.ReportedBy, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ReportedByName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.ReportReason, opt => opt.MapFrom(src => (short)1)) // Default to first reason since entity doesn't have ReportReason
            .ForMember(dest => dest.ReportReasonName, opt => opt.MapFrom(src => GetReportReasonName(1)))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Reason))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Processed ? (short)3 : (short)1)) // 3: Resolved if processed, 1: Pending if not
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetReportStatusName(src.Processed ? (short)3 : (short)1)))
            .ForMember(dest => dest.ResolvedAt, opt => opt.MapFrom(src => src.ProcessedAt))
            .ForMember(dest => dest.ResolvedBy, opt => opt.MapFrom(src => src.ProcessedBy))
            .ForMember(dest => dest.ResolvedByName, opt => opt.MapFrom(src => src.ProcessedByUser != null ? $"{src.ProcessedByUser.FirstName} {src.ProcessedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.ResolutionNotes, opt => opt.MapFrom(src => (string?)null)); // Entity doesn't have ResolutionNotes

        CreateMap<CreateReviewReportRequest, ReviewReport>()
            .ForMember(dest => dest.ReportId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ProductReviewId))
            .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateReviewReportRequest, ReviewReport>()
            .ForMember(dest => dest.Processed, opt => opt.MapFrom(src => src.Status == 3)) // Set processed based on status
            .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.Status == 3 ? DateTime.UtcNow : (DateTime?)null));
    }

    private static string GetReviewStatusName(short status)
    {
        return status switch
        {
            1 => "Pending",
            2 => "Approved",
            3 => "Rejected",
            4 => "Hidden",
            _ => "Unknown"
        };
    }

    private static string GetReportReasonName(short reportReason)
    {
        return reportReason switch
        {
            1 => "Spam",
            2 => "Inappropriate Content",
            3 => "Fake Review",
            4 => "Offensive Language",
            5 => "Irrelevant",
            6 => "Other",
            _ => "Unknown"
        };
    }

    private static string GetReportStatusName(short status)
    {
        return status switch
        {
            1 => "Pending",
            2 => "Under Review",
            3 => "Resolved",
            4 => "Dismissed",
            _ => "Unknown"
        };
    }
}

