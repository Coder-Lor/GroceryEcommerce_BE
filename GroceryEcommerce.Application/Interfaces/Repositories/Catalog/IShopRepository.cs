using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IShopRepository
{
    // Basic CRUD
    Task<Result<Shop?>> GetByIdAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<Result<Shop?>> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Shop>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<Shop>> CreateAsync(Shop shop, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(Shop shop, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid shopId, CancellationToken cancellationToken = default);

    // Business queries
    Task<Result<PagedResult<Shop>>> GetByOwnerAsync(Guid ownerUserId, PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Shop>>> GetActiveShopsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid shopId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsBySlugAsync(string slug, CancellationToken cancellationToken = default);
}


