using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Sales;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Sales;

public class OrderShipmentRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderShipmentRepository> logger
) : BasePagedRepository<OrderShipmentEntity, OrderShipment>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IOrderShipmentRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("ShipmentNumber", typeof(string)),
            new SearchableField("TrackingNumber", typeof(string)),
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("CarrierId", typeof(Guid)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("CreatedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "CreatedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "ShipmentNumber", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TrackingNumber", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CarrierId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "shipmentnumber", OrderShipmentFields.ShipmentNumber },
            { "trackingnumber", OrderShipmentFields.TrackingNumber },
            { "orderid", OrderShipmentFields.OrderId },
            { "carrierid", OrderShipmentFields.CarrierId },
            { "status", OrderShipmentFields.Status },
            { "createdat", OrderShipmentFields.CreatedAt }
        };
    }

    protected override EntityQuery<OrderShipmentEntity> ApplySearch(EntityQuery<OrderShipmentEntity> query, string searchTerm)
    {
        return query.Where(
            OrderShipmentFields.ShipmentNumber.Contains(searchTerm) |
            OrderShipmentFields.TrackingNumber.Contains(searchTerm)
        );
    }

    protected override EntityQuery<OrderShipmentEntity> ApplySorting(EntityQuery<OrderShipmentEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "shipmentnumber" => OrderShipmentFields.ShipmentNumber,
            "trackingnumber" => OrderShipmentFields.TrackingNumber,
            "orderid" => OrderShipmentFields.OrderId,
            "carrierid" => OrderShipmentFields.CarrierId,
            "status" => OrderShipmentFields.Status,
            "createdat" => OrderShipmentFields.CreatedAt,
            _ => OrderShipmentFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderShipmentEntity> ApplyDefaultSorting(EntityQuery<OrderShipmentEntity> query)
    {
        return query.OrderBy(OrderShipmentFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderShipmentFields.ShipmentId;
    }

    protected override object GetEntityId(OrderShipmentEntity entity, EntityField2 primaryKeyField)
    {
        return entity.ShipmentId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderShipmentFields.ShipmentId.In(ids);
    }

    public async Task<Result<OrderShipment?>> GetByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderShipmentFields.ShipmentId, shipmentId, "OrderShipment", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<OrderShipment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderShipmentEntity>()
                .Where(OrderShipmentFields.OrderId == orderId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderShipment>>(entities);
            
            return Result<List<OrderShipment>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order shipments for order: {OrderId}", orderId);
            return Result<List<OrderShipment>>.Failure("An error occurred while fetching order shipments.");
        }
    }

    public async Task<Result<OrderShipment?>> GetByShipmentNumberAsync(string shipmentNumber, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderShipmentFields.ShipmentNumber, shipmentNumber, "OrderShipment_Number", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<OrderShipment?>> GetByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderShipmentFields.TrackingNumber, trackingNumber, "OrderShipment_Tracking", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<bool>> CreateAsync(OrderShipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderShipmentEntity>(shipment);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderShipment*", cancellationToken);
            
            Logger.LogInformation("Order shipment created: {ShipmentId}", entity.ShipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order shipment");
            return Result<bool>.Failure("An error occurred while creating order shipment.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(OrderShipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderShipmentEntity>(shipment);
            entity.IsNew = false;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderShipment*", cancellationToken);
            
            Logger.LogInformation("Order shipment updated: {ShipmentId}", entity.ShipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order shipment: {ShipmentId}", shipment.ShipmentId);
            return Result<bool>.Failure("An error occurred while updating order shipment.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderShipmentEntity(shipmentId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderShipment*", cancellationToken);
            
            Logger.LogInformation("Order shipment deleted: {ShipmentId}", shipmentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order shipment: {ShipmentId}", shipmentId);
            return Result<bool>.Failure("An error occurred while deleting order shipment.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(OrderShipmentFields.ShipmentId, shipmentId, cancellationToken);
    }

    public async Task<Result<PagedResult<OrderShipment>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", status),
            OrderShipmentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderShipment>>> GetByCarrierAsync(Guid carrierId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("CarrierId", carrierId),
            OrderShipmentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderShipment>>> GetByWarehouseAsync(Guid warehouseId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        // Note: WarehouseId might not exist in OrderShipment, this is a placeholder
        // You may need to adjust based on your actual schema
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("CarrierId", warehouseId), // Placeholder
            OrderShipmentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderShipment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithRangeFilter("CreatedAt", fromDate, toDate),
            OrderShipmentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<bool>> UpdateShipmentStatusAsync(Guid shipmentId, short status, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderShipmentEntity>()
                .Where(OrderShipmentFields.ShipmentId == shipmentId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                return Result<bool>.Failure("Shipment not found.");
            }
            
            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            
            if (status == 2) // Shipped
            {
                entity.ShippedAt = DateTime.UtcNow;
            }
            else if (status == 4) // Delivered
            {
                entity.DeliveredAt = DateTime.UtcNow;
            }
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderShipment*", cancellationToken);
            
            Logger.LogInformation("Order shipment status updated: {ShipmentId}, Status: {Status}", shipmentId, status);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating shipment status: {ShipmentId}", shipmentId);
            return Result<bool>.Failure("An error occurred while updating shipment status.");
        }
    }

    public async Task<Result<string>> GenerateShipmentNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<OrderShipmentEntity>()
                .OrderBy(OrderShipmentFields.CreatedAt.Descending())
                .Limit(1);
            
            var lastShipment = await adapter.FetchFirstAsync(query, cancellationToken);
            
            var shipmentNumber = $"SHIP-{DateTime.UtcNow:yyyyMMdd}-{GenerateSequence(lastShipment?.ShipmentNumber)}";
            
            Logger.LogInformation("Generated shipment number: {ShipmentNumber}", shipmentNumber);
            return Result<string>.Success(shipmentNumber);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating shipment number");
            return Result<string>.Failure("An error occurred while generating shipment number.");
        }
    }

    private static string GenerateSequence(string? lastShipmentNumber)
    {
        if (string.IsNullOrEmpty(lastShipmentNumber))
            return "0001";
        
        var parts = lastShipmentNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var sequence))
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            if (parts[1] == today)
            {
                return (sequence + 1).ToString("D4");
            }
        }
        
        return "0001";
    }
}

