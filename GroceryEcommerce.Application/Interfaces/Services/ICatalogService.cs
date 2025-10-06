using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models;
using GroceryEcommerce.Application.Models.Catalog;

namespace GroceryEcommerce.Application.Interfaces.Services;

public interface ICatalogService
{
    // Category services
    Task<Result<List<CategoryDto>>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<Result<CategoryDto?>> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto?>> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<List<CategoryDto>>> GetSubCategoriesAsync(Guid parentCategoryId, CancellationToken cancellationToken = default);
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);

    // Brand services
    Task<Result<List<BrandDto>>> GetBrandsAsync(CancellationToken cancellationToken = default);
    Task<Result<BrandDto?>> GetBrandByIdAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<BrandDto?>> GetBrandBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<BrandDto>> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateBrandAsync(Guid brandId, UpdateBrandRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteBrandAsync(Guid brandId, CancellationToken cancellationToken = default);

    // Product services
    Task<Result<PagedResult<ProductDto>>> GetProductsAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductDetailDto?>> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductDetailDto?>> GetProductBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Result<ProductDto?>> GetProductBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Result<List<ProductDto>>> GetProductsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductDto>>> GetProductsByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
    Task<Result<List<ProductDto>>> GetFeaturedProductsAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<Result<List<ProductDto>>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductAsync(Guid productId, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task<Result<bool>> ToggleProductFeaturedAsync(Guid productId, CancellationToken cancellationToken = default);

    // Product Image services
    Task<Result<List<ProductImageDto>>> GetProductImagesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductImageDto>> CreateProductImageAsync(CreateProductImageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductImageAsync(Guid imageId, UpdateProductImageRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductImageAsync(Guid imageId, CancellationToken cancellationToken = default);
    Task<Result<bool>> SetPrimaryImageAsync(Guid productId, Guid imageId, CancellationToken cancellationToken = default);

    // Product Variant services
    Task<Result<List<ProductVariantDto>>> GetProductVariantsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariantDto?>> GetProductVariantByIdAsync(Guid variantId, CancellationToken cancellationToken = default);
    Task<Result<ProductVariantDto>> CreateProductVariantAsync(CreateProductVariantRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductVariantAsync(Guid variantId, UpdateProductVariantRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductVariantAsync(Guid variantId, CancellationToken cancellationToken = default);

    // Product Attribute services
    Task<Result<List<ProductAttributeDto>>> GetProductAttributesAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductAttributeDto?>> GetProductAttributeByIdAsync(Guid attributeId, CancellationToken cancellationToken = default);
    Task<Result<ProductAttributeDto>> CreateProductAttributeAsync(CreateProductAttributeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductAttributeAsync(Guid attributeId, UpdateProductAttributeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductAttributeAsync(Guid attributeId, CancellationToken cancellationToken = default);

    // Product Tag services
    Task<Result<List<ProductTagDto>>> GetProductTagsAsync(CancellationToken cancellationToken = default);
    Task<Result<ProductTagDto?>> GetProductTagByIdAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<ProductTagDto>> CreateProductTagAsync(CreateProductTagRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductTagAsync(Guid tagId, UpdateProductTagRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignTagToProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RemoveTagFromProductAsync(Guid productId, Guid tagId, CancellationToken cancellationToken = default);

    // Product Question services
    Task<Result<List<ProductQuestionDto>>> GetProductQuestionsAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<ProductQuestionDto?>> GetProductQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Result<ProductQuestionDto>> CreateProductQuestionAsync(CreateProductQuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateProductQuestionAsync(Guid questionId, UpdateProductQuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteProductQuestionAsync(Guid questionId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AnswerProductQuestionAsync(Guid questionId, string answer, CancellationToken cancellationToken = default);
}
