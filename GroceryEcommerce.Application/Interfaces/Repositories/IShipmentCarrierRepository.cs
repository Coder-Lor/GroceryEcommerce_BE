using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Domain.Entities.Sales;

namespace GroceryEcommerce.Application.Interfaces.Repositories;

public interface IShipmentCarrierRepository
{
    // Basic CRUD operations
    Task<Result<ShipmentCarrier?>> GetByIdAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrier?>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrier?>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<List<ShipmentCarrier>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PagedResult<ShipmentCarrier>>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<Result<ShipmentCarrier>> CreateAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid carrierId, CancellationToken cancellationToken = default);
    
    // Carrier management operations
    Task<Result<bool>> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Result<bool>> ExistsAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<List<ShipmentCarrier>>> GetActiveCarriersAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> IsCarrierInUseAsync(Guid carrierId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetShipmentCountByCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default);
}
