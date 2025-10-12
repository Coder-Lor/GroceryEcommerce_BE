using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Repositories.Catalog;

public interface ICatalogRepository
{
    // Product Image operations
    
    Task<Result<ProductImage>> CreateProductImageAsync(ProductImage productImage, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductImageAsync(ProductImage productImage, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductImageAsync(Guid productImageId, CancellationToken cancellationToken = default);

    // Product Variant operations
    Task<Result<List<ProductVariant>>> GetProductVariantsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariant?>> GetProductVariantByIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariant>> CreateProductVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductVariantAsync(ProductVariant variant, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductVariantAsync(Guid variantId, CancellationToken cancellationToken = default);

    // Product Attribute operations
    Task<Result<List<ProductAttribute>>> GetProductAttributesAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductAttribute?>> GetProductAttributeByIdAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttribute>> CreateProductAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductAttributeAsync(ProductAttribute attribute, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default);

    // Product Attribute Value operations
    Task<Result<List<ProductAttributeValue>>> GetProductAttributeValuesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttributeValue>> CreateProductAttributeValueAsync(ProductAttributeValue attributeValue, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductAttributeValueAsync(ProductAttributeValue attributeValue, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductAttributeValueAsync(Guid attributeValueId, CancellationToken cancellationToken = default);

    // Product Tag operations
    Task<Result<List<ProductTag>>> GetProductTagsAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductTag?>> GetProductTagByIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<ProductTag>> CreateProductTagAsync(ProductTag tag, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductTagAsync(ProductTag tag, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductTagAsync(Guid tagId, CancellationToken cancellationToken = default);

    // Product Tag Assignment operations
    Task<Result<List<ProductTagAssignment>>> GetProductTagAssignmentsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductTagAssignment>> CreateProductTagAssignmentAsync(ProductTagAssignment assignment, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductTagAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    // Product Question operations
    Task<Result<List<ProductQuestion>>> GetProductQuestionsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductQuestion?>> GetProductQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Result<ProductQuestion>> CreateProductQuestionAsync(ProductQuestion question, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductQuestionAsync(ProductQuestion question, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductQuestionAsync(Guid questionId, CancellationToken cancellationToken = default);
}
