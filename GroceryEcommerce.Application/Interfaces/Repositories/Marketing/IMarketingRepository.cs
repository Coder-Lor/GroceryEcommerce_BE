using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Marketing;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Marketing;

public interface IMarketingRepository
{
    // Coupon operations
    Task<Result<List<Coupon>>> GetCouponsAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Coupon>>> GetCouponsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Coupon?>> GetCouponByIdAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<Coupon?>> GetCouponByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<Coupon>>> GetActiveCouponsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<Coupon>>> GetCouponsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<Coupon>> CreateCouponAsync(Coupon coupon, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCouponAsync(Coupon coupon, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCouponAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateCouponAsync(string code, decimal orderAmount, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<Result<decimal>> CalculateCouponDiscountAsync(string code, decimal orderAmount, CancellationToken cancellationToken = default);

    // Coupon Usage operations
    Task<Result<List<CouponUsage>>> GetCouponUsagesAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<List<CouponUsage>>> GetCouponUsagesByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<CouponUsage?>> GetCouponUsageByIdAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<CouponUsage>> CreateCouponUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCouponUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCouponUsageAsync(Guid usageId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCouponUsageCountAsync(Guid couponId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserCouponUsageCountAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default);

    // Gift Card operations
    Task<Result<List<GiftCard>>> GetGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<GiftCard>>> GetGiftCardsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<GiftCard?>> GetGiftCardByIdAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<GiftCard?>> GetGiftCardByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetGiftCardsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetActiveGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<GiftCard>> CreateGiftCardAsync(GiftCard giftCard, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateGiftCardAsync(GiftCard giftCard, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteGiftCardAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ValidateGiftCardAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetGiftCardBalanceAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<bool>> RedeemGiftCardAsync(string code, decimal amount, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateGiftCardCodeAsync(CancellationToken cancellationToken = default);

    // Reward Point operations
    Task<Result<List<RewardPoint>>> GetRewardPointsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<RewardPoint>>> GetRewardPointsPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<RewardPoint?>> GetRewardPointByIdAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<RewardPoint>> CreateRewardPointAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateRewardPointAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteRewardPointAsync(Guid rewardPointId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserTotalRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUserAvailableRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AddRewardPointsAsync(Guid userId, int points, string reason, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeductRewardPointsAsync(Guid userId, int points, string reason, CancellationToken cancellationToken = default);

    // Marketing analytics
    Task<Result<List<object>>> GetCouponUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetGiftCardUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<List<object>>> GetRewardPointStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalCouponDiscountsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalGiftCardRedemptionsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
}
