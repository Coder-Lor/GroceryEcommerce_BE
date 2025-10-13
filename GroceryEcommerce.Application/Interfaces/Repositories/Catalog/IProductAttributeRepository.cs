using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductAttributeRepository
{
    // Basic CRUD operations
    Task<Result<ProductAttribute?>> GetByIdAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttribute?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttribute>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ProductAttribute>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductAttribute>> CreateAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid attributeId, CancellationToken cancellationToken = default);
    
    // Attribute management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttribute>>> GetRequiredAttributesAsync(CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttribute>>> GetByTypeAsync(short attributeType, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsAttributeInUseAsync(Guid attributeId, CancellationToken cancellationToken = default);
}
