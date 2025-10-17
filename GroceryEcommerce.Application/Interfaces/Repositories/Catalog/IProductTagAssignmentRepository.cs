using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductTagAssignmentRepository
{
    // Basic CRUD operations
    Task<Result<ProductTagAssignment?>> GetByIdAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductTagAssignment>>> GetByProductIdAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductTagAssignment>>> GetByTagIdAsync(PagedRequest request, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<ProductTagAssignment>> CreateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    
    // Tag assignment management operations
    Task<Result<bool>> ExistsAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignTagToProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveTagFromProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveAllTagsFromProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Guid>>> GetProductIdsByTagAsync(PagedRequest request, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Guid>>> GetTagIdsByProductAsync(PagedRequest request, Guid productId, CancellationToken cancellationToken = default);
}
