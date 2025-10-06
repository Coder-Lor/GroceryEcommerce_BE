using AutoMapper;
using GroceryEcommerce.Application.Models.Marketing;
using GroceryEcommerce.Domain.Entities.Marketing;

namespace GroceryEcommerce.Application.Mapping;

public class MarketingProfile : Profile
{
    public MarketingProfile()
    {
        // Coupon mappings
        CreateMap<Coupon, CouponDto>()
            .ForMember(dest => dest.DiscountTypeName, opt => opt.MapFrom(src => GetDiscountTypeName(src.DiscountType)))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetCouponStatusName(src.Status)))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.Status == 1 && src.ValidFrom <= DateTime.UtcNow && src.ValidTo >= DateTime.UtcNow))
            .ForMember(dest => dest.ValidationMessage, opt => opt.MapFrom(src => GetCouponValidationMessage(src)));

        CreateMap<CreateCouponRequest, Coupon>()
            .ForMember(dest => dest.CouponId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateCouponRequest, Coupon>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Coupon Usage mappings
        CreateMap<CouponUsage, CouponUsageDto>()
            .ForMember(dest => dest.CouponUsageId, opt => opt.MapFrom(src => src.UsageId))
            .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.Coupon.Code))
            .ForMember(dest => dest.CouponName, opt => opt.MapFrom(src => src.Coupon.Name))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Order.OrderNumber))
            .ForMember(dest => dest.OrderAmount, opt => opt.MapFrom(src => src.Order.TotalAmount));

        CreateMap<CreateCouponUsageRequest, CouponUsage>()
            .ForMember(dest => dest.UsageId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.UsedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Gift Card mappings
        CreateMap<GiftCard, GiftCardDto>()
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => GetGiftCardStatusName(src.IsActive ? (short)1 : (short)0)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? (short)1 : (short)0))
            .ForMember(dest => dest.CurrentBalance, opt => opt.MapFrom(src => src.Balance))
            .ForMember(dest => dest.UsedAmount, opt => opt.MapFrom(src => src.InitialAmount - src.Balance))
            .ForMember(dest => dest.ValidFrom, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ValidTo, opt => opt.MapFrom(src => src.ExpiresAt ?? DateTime.MaxValue))
            .ForMember(dest => dest.PurchasedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null))
            .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.AssignedToEmail, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.ActivatedAt, opt => opt.MapFrom(src => (DateTime?)null))
            .ForMember(dest => dest.ExpiredAt, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsActive && (src.ExpiresAt == null || src.ExpiresAt > DateTime.UtcNow) && src.Balance > 0))
            .ForMember(dest => dest.ValidationMessage, opt => opt.MapFrom(src => GetGiftCardValidationMessage(src)))
            .ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => new List<GiftCardTransactionDto>()));

        CreateMap<CreateGiftCardRequest, GiftCard>()
            .ForMember(dest => dest.GiftCardId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialAmount))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateGiftCardRequest, GiftCard>()
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.InitialAmount));

        // Gift Card Transaction mappings - Commented out since GiftCardTransaction entity doesn't exist
        // CreateMap<GiftCardTransaction, GiftCardTransactionDto>()
        //     .ForMember(dest => dest.GiftCardCode, opt => opt.MapFrom(src => src.GiftCard.Code))
        //     .ForMember(dest => dest.TransactionTypeName, opt => opt.MapFrom(src => GetTransactionTypeName(src.TransactionType)))
        //     .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser != null ? $"{src.CreatedByUser.FirstName} {src.CreatedByUser.LastName}".Trim() : null));

        // Reward Point mappings
        CreateMap<RewardPoint, RewardPointDto>()
            .ForMember(dest => dest.RewardPointId, opt => opt.MapFrom(src => src.RewardId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()))
            .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => (short)1)) // Default to "Earned"
            .ForMember(dest => dest.TransactionTypeName, opt => opt.MapFrom(src => GetRewardTransactionTypeName(1)))
            .ForMember(dest => dest.BalanceBefore, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.BalanceAfter, opt => opt.MapFrom(src => src.Points))
            .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => (string?)null))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiresAt.HasValue && src.ExpiresAt.Value < DateTime.UtcNow));

        CreateMap<CreateRewardPointRequest, RewardPoint>()
            .ForMember(dest => dest.RewardId, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
    }

    private static string GetDiscountTypeName(short discountType)
    {
        return discountType switch
        {
            1 => "Percentage",
            2 => "Fixed Amount",
            _ => "Unknown"
        };
    }

    private static string GetCouponStatusName(short status)
    {
        return status switch
        {
            1 => "Active",
            2 => "Inactive",
            3 => "Expired",
            4 => "Suspended",
            _ => "Unknown"
        };
    }

    private static string GetGiftCardStatusName(short status)
    {
        return status switch
        {
            1 => "Active",
            2 => "Used",
            3 => "Expired",
            4 => "Suspended",
            _ => "Unknown"
        };
    }

    private static string GetTransactionTypeName(short transactionType)
    {
        return transactionType switch
        {
            1 => "Purchase",
            2 => "Redemption",
            3 => "Refund",
            4 => "Adjustment",
            _ => "Unknown"
        };
    }

    private static string GetRewardTransactionTypeName(short transactionType)
    {
        return transactionType switch
        {
            1 => "Earned",
            2 => "Redeemed",
            3 => "Expired",
            4 => "Adjusted",
            _ => "Unknown"
        };
    }

    private static string GetCouponValidationMessage(Coupon coupon)
    {
        if (coupon.Status != 1) return "Coupon is not active";
        if (coupon.ValidFrom > DateTime.UtcNow) return "Coupon is not yet valid";
        if (coupon.ValidTo < DateTime.UtcNow) return "Coupon has expired";
        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit.Value) return "Coupon usage limit reached";
        return null;
    }

    private static string GetGiftCardValidationMessage(GiftCard giftCard)
    {
        if (!giftCard.IsActive) return "Gift card is not active";
        if (giftCard.ExpiresAt.HasValue && giftCard.ExpiresAt.Value < DateTime.UtcNow) return "Gift card has expired";
        if (giftCard.Balance <= 0) return "Gift card has no balance";
        return null;
    }
}

