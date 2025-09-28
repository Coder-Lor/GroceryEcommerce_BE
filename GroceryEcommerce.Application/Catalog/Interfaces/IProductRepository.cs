using GroceryEcommerce.Application.Common.Interfaces;

namespace GroceryEcommerce.Application.Catalog.Interfaces
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
        Task<ProductEntity?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetByBrandAsync(Guid brandId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductEntity>> GetLowStockProductsAsync(CancellationToken cancellationToken = default);
    }
}