using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IProductTagAssignmentRepository
{
    // Basic CRUD operations
    Task<Result<ProductTagAssignment?>> GetByIdAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductTagAssignment>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductTagAssignment>>> GetByTagIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<ProductTagAssignment>> CreateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    
    // Tag assignment management operations
    Task<Result<bool>> ExistsAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignTagToProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveTagFromProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveAllTagsFromProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<Guid>>> GetProductIdsByTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<List<Guid>>> GetTagIdsByProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
