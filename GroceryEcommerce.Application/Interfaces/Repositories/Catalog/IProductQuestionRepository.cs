using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductQuestionRepository
{
    // Basic CRUD operations
    Task<Result<ProductQuestion?>> GetByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductQuestion>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductQuestion>>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductQuestion>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductQuestion>> CreateAsync(ProductQuestion question, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductQuestion question, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid questionId, CancellationToken cancellationToken = default);
    
    // Question management operations
    Task<Result<bool>> ExistsAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductQuestion>>> GetUnansweredQuestionsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ProductQuestion>>> GetAnsweredQuestionsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ProductQuestion>>> GetByStatusAsync(short status, CancellationToken cancellationToken = default);
    Task<Result<bool>> AnswerQuestionAsync(Guid questionId, string answer, Guid answeredBy, CancellationToken cancellationToken = default);
    Task<Result<int>> GetQuestionCountByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetUnansweredCountAsync(CancellationToken cancellationToken = default);
}
