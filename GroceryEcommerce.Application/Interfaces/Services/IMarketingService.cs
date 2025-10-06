using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Marketing;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface IMarketingService
{
    // Coupon services
    Task<Result<List<CouponDto>>> GetCouponsAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<CouponDto>>> GetCouponsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<CouponDto?>> GetCouponByIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<CouponDto?>> GetCouponByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<CouponDto>>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<CouponDto>>> GetCouponsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<CouponDto>> CreateCouponAsync(CreateCouponRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCouponAsync(Guid couponId, UpdateCouponRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCouponAsync(ValidateCouponRequest request, CancellationToken cancellationToken = default);
    Task<Result<decimal>> CalculateCouponDiscountAsync(string code, decimal orderAmount, CancellationToken cancellationToken = default);

    // Coupon Usage services
    Task<Result<List<CouponUsageDto>>> GetCouponUsagesAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsageDto>>> GetCouponUsagesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CouponUsageDto?>> GetCouponUsageByIdAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<CouponUsageDto>> CreateCouponUsageAsync(CreateCouponUsageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCouponUsageAsync(Guid usageId, CreateCouponUsageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCouponUsageAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCouponUsageCountAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserCouponUsageCountAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default);

    // Gift Card services
    Task<Result<List<GiftCardDto>>> GetGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<GiftCardDto>>> GetGiftCardsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<GiftCardDto?>> GetGiftCardByIdAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<GiftCardDto?>> GetGiftCardByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCardDto>>> GetGiftCardsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCardDto>>> GetActiveGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<GiftCardDto>> CreateGiftCardAsync(CreateGiftCardRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateGiftCardAsync(Guid giftCardId, UpdateGiftCardRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteGiftCardAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateGiftCardAsync(ValidateGiftCardRequest request, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetGiftCardBalanceAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<bool>> RedeemGiftCardAsync(RedeemGiftCardRequest request, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateGiftCardCodeAsync(CancellationToken cancellationToken = default);

    // Reward Point services
    Task<Result<List<RewardPointDto>>> GetRewardPointsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<RewardPointDto>>> GetRewardPointsPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<RewardPointDto?>> GetRewardPointByIdAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<RewardPointDto>> CreateRewardPointAsync(CreateRewardPointRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateRewardPointAsync(Guid rewardPointId, UpdateRewardPointRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteRewardPointAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserTotalRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserAvailableRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddRewardPointsAsync(AddRewardPointsRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeductRewardPointsAsync(DeductRewardPointsRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserRewardSummaryDto>> GetUserRewardSummaryAsync(Guid userId, CancellationToken cancellationToken = default);

    // Marketing analytics
    Task<Result<List<object>>> GetCouponUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetGiftCardUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetRewardPointStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalCouponDiscountsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalGiftCardRedemptionsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
