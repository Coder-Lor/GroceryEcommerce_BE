using GroceryEcommerce.Application.Common.Interfaces;

namespace GroceryEcommerce.Application.Catalog.Interfaces
{
    public interface IBrandRepository : IRepository<BrandEntity>
    {
        Task<BrandEntity?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<IEnumerable<BrandEntity>> GetActiveBrandsAsync(CancellationToken cancellationToken = default);
    }
}