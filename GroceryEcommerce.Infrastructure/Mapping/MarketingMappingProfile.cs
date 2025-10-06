using AutoMapper;
using GroceryEcommerce.Domain.Entities.Marketing;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class MarketingMappingProfile : Profile
{
    public MarketingMappingProfile()
    {
        // Coupon mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<Coupon, CouponEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore())
            .ForMember(dest => dest.CouponUsages, opt => opt.Ignore());

        CreateMap<CouponEntity, Coupon>()
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore())
            .ForMember(dest => dest.CouponUsages, opt => opt.Ignore());

        // CouponUsage mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<CouponUsage, CouponUsageEntity>()
            .ForMember(dest => dest.Coupon, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore());

        CreateMap<CouponUsageEntity, CouponUsage>()
            .ForMember(dest => dest.Coupon, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore());

        // GiftCard mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<GiftCard, GiftCardEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<GiftCardEntity, GiftCard>()
            .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore());

        // RewardPoint mapping - AutoMapper tự động map các thuộc tính có tên giống nhau
        CreateMap<RewardPoint, RewardPointEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<RewardPointEntity, RewardPoint>()
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
