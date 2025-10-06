using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IProductImageRepository
{
    // Basic CRUD operations
    Task<Result<ProductImage?>> GetByIdAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductImage>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductImage>> CreateAsync(ProductImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductImage image, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid imageId, CancellationToken cancellationToken = default);
    
    // Image management operations
    Task<Result<bool>> ExistsAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<ProductImage?>> GetPrimaryImageByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetPrimaryImageAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemovePrimaryImageAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductImage>>> GetImagesByTypeAsync(Guid productId, short imageType, CancellationToken cancellationToken = default);
    Task<Result<int>> GetImageCountByProductAsync(Guid productId, CancellationToken cancellationToken = default);
}
