using AutoMapper;
using GroceryEcommerce.Domain.Entities.Reviews;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class ReviewsMappingProfile : Profile
{
    public ReviewsMappingProfile()
    {
        // ProductReview mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ProductReview, ProductReviewEntity>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewImages, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewReports, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewVotes, opt => opt.Ignore());

        CreateMap<ProductReviewEntity, ProductReview>()
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewImages, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewReports, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewVotes, opt => opt.Ignore());

        // ReviewImage mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ReviewImage, ReviewImageEntity>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore());

        CreateMap<ReviewImageEntity, ReviewImage>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore());

        // ReviewReport mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ReviewReport, ReviewReportEntity>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore());

        CreateMap<ReviewReportEntity, ReviewReport>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ProcessedByUser, opt => opt.Ignore());

        // ReviewVote mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<ReviewVote, ReviewVoteEntity>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<ReviewVoteEntity, ReviewVote>()
            .ForMember(dest => dest.ProductReview, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
