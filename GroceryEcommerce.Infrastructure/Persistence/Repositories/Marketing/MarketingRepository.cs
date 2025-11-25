using System.Linq;
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Marketing;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Marketing;

public class MarketingRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ICouponUsageRepository couponUsageRepository,
    ILogger<MarketingRepository> logger
) : IMarketingRepository
{
    protected DataAccessAdapter GetAdapter() => scopedAdapter;
    protected IMapper Mapper => mapper;
    protected ICacheService CacheService => cacheService;
    protected ILogger<MarketingRepository> Logger => logger;
    protected ICouponUsageRepository CouponUsageRepository => couponUsageRepository;

    // Coupon operations
    public async Task<Result<List<Coupon>>> GetCouponsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponEntity>()
                .OrderBy(CouponFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<Coupon>>(entities);
            
            return Result<List<Coupon>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all coupons");
            return Result<List<Coupon>>.Failure("An error occurred while fetching coupons.");
        }
    }

    public async Task<Result<PagedResult<Coupon>>> GetCouponsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponEntity>();
            
            // Apply search if provided
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(CouponFields.Code.Contains(request.Search) | 
                                   CouponFields.Name.Contains(request.Search));
            }
            
            // Get total count
            var countQuery = query.Select(() => Functions.CountRow());
            var totalCount = await adapter.FetchScalarAsync<int>(countQuery, cancellationToken);
            
            // Apply pagination
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 10;
            var skip = (page - 1) * pageSize;
            
            query = query.OrderBy(CouponFields.CreatedAt.Descending())
                        .Limit(pageSize)
                        .Offset(skip);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<Coupon>>(entities);
            
            var result = new PagedResult<Coupon>(domainEntities, totalCount, page, pageSize);
            return Result<PagedResult<Coupon>>.Success(result);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting paged coupons");
            return Result<PagedResult<Coupon>>.Failure("An error occurred while fetching coupons.");
        }
    }

    public async Task<Result<Coupon?>> GetCouponByIdAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponEntity>()
                .Where(CouponFields.CouponId == couponId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                Logger.LogWarning("Coupon not found: {CouponId}", couponId);
                return Result<Coupon?>.Success(null);
            }
            
            var domainEntity = Mapper.Map<Coupon>(entity);
            return Result<Coupon?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting coupon by ID: {CouponId}", couponId);
            return Result<Coupon?>.Failure("An error occurred while fetching coupon.");
        }
    }

    public async Task<Result<Coupon?>> GetCouponByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponEntity>()
                .Where(CouponFields.Code == code);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                Logger.LogWarning("Coupon not found by code: {Code}", code);
                return Result<Coupon?>.Success(null);
            }
            
            var domainEntity = Mapper.Map<Coupon>(entity);
            return Result<Coupon?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting coupon by code: {Code}", code);
            return Result<Coupon?>.Failure("An error occurred while fetching coupon.");
        }
    }

    public async Task<Result<List<Coupon>>> GetActiveCouponsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var now = DateTime.UtcNow;
            var query = qf.Create<CouponEntity>()
                .Where(CouponFields.Status == 1 & 
                       CouponFields.ValidFrom <= now & 
                       CouponFields.ValidTo >= now)
                .OrderBy(CouponFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<Coupon>>(entities);
            
            return Result<List<Coupon>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting active coupons");
            return Result<List<Coupon>>.Failure("An error occurred while fetching active coupons.");
        }
    }

    public async Task<Result<List<Coupon>>> GetCouponsByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<CouponEntity>()
                .Where(CouponFields.ValidFrom >= fromDate & CouponFields.ValidTo <= toDate)
                .OrderBy(CouponFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<Coupon>>(entities);
            
            return Result<List<Coupon>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting coupons by date range");
            return Result<List<Coupon>>.Failure("An error occurred while fetching coupons.");
        }
    }

    public async Task<Result<Coupon>> CreateCouponAsync(Coupon coupon, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<CouponEntity>(coupon);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Coupon*", cancellationToken);
            
            Logger.LogInformation("Coupon created: {CouponId}", entity.CouponId);
            var domainEntity = Mapper.Map<Coupon>(entity);
            return Result<Coupon>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating coupon");
            return Result<Coupon>.Failure("An error occurred while creating coupon.");
        }
    }

    public async Task<Result<bool>> UpdateCouponAsync(Coupon coupon, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<CouponEntity>(coupon);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Coupon*", cancellationToken);
            
            Logger.LogInformation("Coupon updated: {CouponId}", entity.CouponId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating coupon: {CouponId}", coupon.CouponId);
            return Result<bool>.Failure("An error occurred while updating coupon.");
        }
    }

    public async Task<Result<bool>> DeleteCouponAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new CouponEntity(couponId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("Coupon*", cancellationToken);
            
            Logger.LogInformation("Coupon deleted: {CouponId}", couponId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting coupon: {CouponId}", couponId);
            return Result<bool>.Failure("An error occurred while deleting coupon.");
        }
    }

    public async Task<Result<bool>> ValidateCouponAsync(string code, decimal orderAmount, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get coupon by code
            var couponResult = await GetCouponByCodeAsync(code, cancellationToken);
            if (!couponResult.IsSuccess || couponResult.Data == null)
            {
                return Result<bool>.Success(false);
            }

            var coupon = couponResult.Data;
            var now = DateTime.UtcNow;

            // Check if coupon is active
            if (coupon.Status != 1)
            {
                Logger.LogWarning("Coupon {Code} is not active", code);
                return Result<bool>.Success(false);
            }

            // Check if coupon is within valid date range
            if (coupon.ValidFrom > now || coupon.ValidTo < now)
            {
                Logger.LogWarning("Coupon {Code} is not within valid date range", code);
                return Result<bool>.Success(false);
            }

            // Check minimum order amount
            if (coupon.MinOrderAmount.HasValue && orderAmount < coupon.MinOrderAmount.Value)
            {
                Logger.LogWarning("Order amount {OrderAmount} is less than minimum order amount {MinOrderAmount} for coupon {Code}", 
                    orderAmount, coupon.MinOrderAmount.Value, code);
                return Result<bool>.Success(false);
            }

            // Check usage limit
            if (coupon.UsageLimit.HasValue)
            {
                var usageCountResult = await CouponUsageRepository.GetUsageCountByCouponAsync(coupon.CouponId, cancellationToken);
                if (usageCountResult.IsSuccess && usageCountResult.Data >= coupon.UsageLimit.Value)
                {
                    Logger.LogWarning("Coupon {Code} has reached usage limit", code);
                    return Result<bool>.Success(false);
                }
            }

            // Check user usage limit
            if (userId.HasValue && coupon.UserUsageLimit > 0)
            {
                var userUsageCountResult = await CouponUsageRepository.GetUsageCountByUserAsync(userId.Value, coupon.CouponId, cancellationToken);
                if (userUsageCountResult.IsSuccess && userUsageCountResult.Data >= coupon.UserUsageLimit)
                {
                    Logger.LogWarning("User {UserId} has reached usage limit for coupon {Code}", userId.Value, code);
                    return Result<bool>.Success(false);
                }
            }

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating coupon: {Code}", code);
            return Result<bool>.Failure("An error occurred while validating coupon.");
        }
    }

    public async Task<Result<decimal>> CalculateCouponDiscountAsync(string code, decimal orderAmount, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get coupon by code
            var couponResult = await GetCouponByCodeAsync(code, cancellationToken);
            if (!couponResult.IsSuccess || couponResult.Data == null)
            {
                return Result<decimal>.Failure("Coupon not found.");
            }

            var coupon = couponResult.Data;
            decimal discount = 0;

            // Calculate discount based on type
            if (coupon.DiscountType == 1) // Percentage
            {
                discount = orderAmount * (coupon.DiscountValue / 100);
                
                // Apply max discount amount if specified
                if (coupon.MaxDiscountAmount.HasValue && discount > coupon.MaxDiscountAmount.Value)
                {
                    discount = coupon.MaxDiscountAmount.Value;
                }
            }
            else if (coupon.DiscountType == 2) // Fixed Amount
            {
                discount = coupon.DiscountValue;
                
                // Don't allow discount to exceed order amount
                if (discount > orderAmount)
                {
                    discount = orderAmount;
                }
            }

            Logger.LogInformation("Calculated discount {Discount} for coupon {Code} on order amount {OrderAmount}", 
                discount, code, orderAmount);
            
            return Result<decimal>.Success(discount);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating coupon discount: {Code}", code);
            return Result<decimal>.Failure("An error occurred while calculating coupon discount.");
        }
    }

    public async Task<Result<List<CouponUsage>>> GetCouponUsagesAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        var result = await CouponUsageRepository.GetByCouponIdAsync(couponId, new PagedRequest { Page = 1, PageSize = int.MaxValue }, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<List<CouponUsage>>.Failure(result.ErrorMessage ?? "Failed to get coupon usages.");
        }
        return Result<List<CouponUsage>>.Success(result.Data.Items.ToList());
    }

    public async Task<Result<List<CouponUsage>>> GetCouponUsagesByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var result = await CouponUsageRepository.GetByUserIdAsync(userId, new PagedRequest { Page = 1, PageSize = int.MaxValue }, cancellationToken);
        if (!result.IsSuccess || result.Data == null)
        {
            return Result<List<CouponUsage>>.Failure(result.ErrorMessage ?? "Failed to get coupon usages.");
        }
        return Result<List<CouponUsage>>.Success(result.Data.Items.ToList());
    }

    public async Task<Result<CouponUsage?>> GetCouponUsageByIdAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.GetByIdAsync(usageId, cancellationToken);
    }

    public async Task<Result<CouponUsage>> CreateCouponUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.CreateAsync(usage, cancellationToken);
    }

    public async Task<Result<bool>> UpdateCouponUsageAsync(CouponUsage usage, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.UpdateAsync(usage, cancellationToken);
    }

    public async Task<Result<bool>> DeleteCouponUsageAsync(Guid usageId, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.DeleteAsync(usageId, cancellationToken);
    }

    public async Task<Result<int>> GetCouponUsageCountAsync(Guid couponId, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.GetUsageCountByCouponAsync(couponId, cancellationToken);
    }

    public async Task<Result<int>> GetUserCouponUsageCountAsync(Guid userId, Guid couponId, CancellationToken cancellationToken = default)
    {
        return await CouponUsageRepository.GetUsageCountByUserAsync(userId, couponId, cancellationToken);
    }

    // Gift Card operations - Not implemented yet, throw NotImplementedException
    public Task<Result<List<GiftCard>>> GetGiftCardsAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<PagedResult<GiftCard>>> GetGiftCardsPagedAsync(PagedRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<GiftCard?>> GetGiftCardByIdAsync(Guid giftCardId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<GiftCard?>> GetGiftCardByCodeAsync(string code, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<List<GiftCard>>> GetGiftCardsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<List<GiftCard>>> GetActiveGiftCardsAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<GiftCard>> CreateGiftCardAsync(GiftCard giftCard, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> UpdateGiftCardAsync(GiftCard giftCard, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> DeleteGiftCardAsync(Guid giftCardId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> ValidateGiftCardAsync(string code, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<decimal>> GetGiftCardBalanceAsync(string code, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> RedeemGiftCardAsync(string code, decimal amount, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<string>> GenerateGiftCardCodeAsync(CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    // Reward Point operations - Not implemented yet, throw NotImplementedException
    public Task<Result<List<RewardPoint>>> GetRewardPointsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<PagedResult<RewardPoint>>> GetRewardPointsPagedAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<RewardPoint?>> GetRewardPointByIdAsync(Guid rewardPointId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<RewardPoint>> CreateRewardPointAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> UpdateRewardPointAsync(RewardPoint rewardPoint, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> DeleteRewardPointAsync(Guid rewardPointId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<int>> GetUserTotalRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<int>> GetUserAvailableRewardPointsAsync(Guid userId, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> AddRewardPointsAsync(Guid userId, int points, string reason, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<bool>> DeductRewardPointsAsync(Guid userId, int points, string reason, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    // Marketing analytics - Not implemented yet, throw NotImplementedException
    public Task<Result<List<object>>> GetCouponUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<List<object>>> GetGiftCardUsageStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<List<object>>> GetRewardPointStatisticsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<decimal>> GetTotalCouponDiscountsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public Task<Result<decimal>> GetTotalGiftCardRedemptionsAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}

