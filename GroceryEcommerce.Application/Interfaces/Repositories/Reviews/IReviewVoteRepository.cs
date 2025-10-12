using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Reviews;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Reviews;

public interface IReviewVoteRepository
{
    // Basic CRUD operations
    Task<Result<ReviewVote?>> GetByIdAsync(Guid voteId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVote?>> GetByReviewAndUserAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewVote>>> GetByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<List<ReviewVote>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<ReviewVote>> CreateAsync(ReviewVote vote, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ReviewVote vote, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid voteId, CancellationToken cancellationToken = default);
    
    // Review vote management operations
    Task<Result<bool>> ExistsAsync(Guid voteId, CancellationToken cancellationToken = default);
    Task<Result<bool>> HasUserVotedAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetHelpfulVoteCountAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetNotHelpfulVoteCountAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<Result<bool>> VoteAsync(Guid reviewId, Guid userId, bool helpful, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveVoteAsync(Guid reviewId, Guid userId, CancellationToken cancellationToken = default);
}
