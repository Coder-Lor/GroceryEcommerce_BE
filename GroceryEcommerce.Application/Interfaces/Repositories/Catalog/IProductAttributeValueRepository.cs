using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface IProductAttributeValueRepository
{
    // Basic CRUD operations
    Task<Result<ProductAttributeValue?>> GetByIdAsync(Guid valueId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttributeValue>>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttributeValue>>> GetByAttributeIdAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttributeValue>> CreateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ProductAttributeValue value, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid valueId, CancellationToken cancellationToken = default);
    
    // Attribute value management operations
    Task<Result<bool>> ExistsAsync(Guid valueId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttributeValue?>> GetByProductAndAttributeAsync(Guid productId, Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteByAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductAttributeValue>>> GetByValueAsync(string value, CancellationToken cancellationToken = default);
}
