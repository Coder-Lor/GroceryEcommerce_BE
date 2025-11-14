using AutoMapper;
using GroceryEcommerce.Domain.Entities.Auth;
using GroceryEcommerce.EntityClasses;

namespace GroceryEcommerce.Infrastructure.Mapping;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserEntity>()
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserAddresses, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoleAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoleAssignments1, opt => opt.Ignore())
            .ForMember(dest => dest.UserSessions, opt => opt.Ignore())
            .ForMember(dest => dest.AbandonedCarts, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCarts, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore())
            .ForMember(dest => dest.Brands, opt => opt.Ignore())
            .ForMember(dest => dest.Brands1, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Categories1, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.Ignore())
            .ForMember(dest => dest.Products1, opt => opt.Ignore())
            .ForMember(dest => dest.ProductQuestions, opt => opt.Ignore())
            .ForMember(dest => dest.ProductQuestions1, opt => opt.Ignore())
            .ForMember(dest => dest.PurchaseOrders, opt => opt.Ignore())
            .ForMember(dest => dest.Coupons, opt => opt.Ignore())
            .ForMember(dest => dest.Coupons1, opt => opt.Ignore())
            .ForMember(dest => dest.CouponUsages, opt => opt.Ignore())
            .ForMember(dest => dest.GiftCards, opt => opt.Ignore())
            .ForMember(dest => dest.RewardPoints, opt => opt.Ignore())
            .ForMember(dest => dest.ProductReviews, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewReports, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewReports1, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewVotes, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.Orders1, opt => opt.Ignore())
            .ForMember(dest => dest.OrderRefunds, opt => opt.Ignore())
            .ForMember(dest => dest.OrderStatusHistories, opt => opt.Ignore())
            .ForMember(dest => dest.SystemSettings, opt => opt.Ignore());

        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.UserRoleAssignments, opt => opt.Ignore())
            .ForMember(dest => dest.UserAddresses, opt => opt.Ignore())
            .ForMember(dest => dest.UserSessions, opt => opt.Ignore())
            .ForMember(dest => dest.AuditLogs, opt => opt.Ignore())
            .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCarts, opt => opt.Ignore())
            .ForMember(dest => dest.Wishlists, opt => opt.Ignore())
            .ForMember(dest => dest.AbandonedCarts, opt => opt.Ignore())
            .ForMember(dest => dest.Orders, opt => opt.Ignore())
            .ForMember(dest => dest.CouponUsages, opt => opt.Ignore())
            .ForMember(dest => dest.RewardPoints, opt => opt.Ignore())
            .ForMember(dest => dest.ProductReviews, opt => opt.Ignore());
        
        CreateMap<RefreshToken, RefreshTokenEntity>()
            .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshTokenValue))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.CreatedByIp, opt => opt.MapFrom(src => src.CreatedByIp))
            .ForMember(dest => dest.ReplacedByToken, opt => opt.MapFrom(src => src.ReplacedByToken));

        CreateMap<RefreshTokenEntity, RefreshToken>()
            .ForMember(dest => dest.RefreshTokenValue, opt => opt.MapFrom(src => src.RefreshToken ?? string.Empty))
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.Revoked, opt => opt.MapFrom(src => src.Revoked))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.CreatedByIp, opt => opt.MapFrom(src => src.CreatedByIp))
            .ForMember(dest => dest.ReplacedByToken, opt => opt.MapFrom(src => src.ReplacedByToken));
        
        CreateMap<UserAddress, UserAddressEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UserAddressEntity, UserAddress>();
        
        CreateMap<UserRole, UserRoleEntity>()
            .ForMember(dest => dest.UserRoleAssignments, opt => opt.Ignore());

        CreateMap<UserRoleEntity, UserRole>();
        
        CreateMap<UserRoleAssignment, UserRoleAssignmentEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.User1, opt => opt.Ignore())
            .ForMember(dest => dest.UserRole, opt => opt.Ignore());

        CreateMap<UserRoleAssignmentEntity, UserRoleAssignment>();
        
        CreateMap<UserSession, UserSessionEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UserSessionEntity, UserSession>();
        
        CreateMap<AuditLog, AuditLogEntity>()
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<AuditLogEntity, AuditLog>();
    }
}
