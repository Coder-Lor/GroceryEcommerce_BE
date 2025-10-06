using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Marketing;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IGiftCardRepository
{
    // Basic CRUD operations
    Task<Result<GiftCard?>> GetByIdAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<GiftCard?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<GiftCard>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<GiftCard>> CreateAsync(GiftCard giftCard, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(GiftCard giftCard, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    
    // Gift card management operations
    Task<Result<bool>> ExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid giftCardId, CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetActiveGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetExpiredGiftCardsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<GiftCard>>> GetGiftCardsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsGiftCardValidAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetRemainingBalanceAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<bool>> RedeemGiftCardAsync(string code, decimal amount, CancellationToken cancellationToken = default);
}
