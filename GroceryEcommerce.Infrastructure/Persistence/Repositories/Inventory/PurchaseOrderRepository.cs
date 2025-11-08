using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Interfaces.Repositories.Inventory;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.DatabaseSpecific;
using GroceryEcommerce.Domain.Entities.Inventory;
using GroceryEcommerce.EntityClasses;
using GroceryEcommerce.FactoryClasses;
using GroceryEcommerce.HelperClasses;
using GroceryEcommerce.Infrastructure.Persistence.Repositories.Common;
using Microsoft.Extensions.Logging;
using SD.LLBLGen.Pro.ORMSupportClasses;
using SD.LLBLGen.Pro.QuerySpec;
using SD.LLBLGen.Pro.QuerySpec.Adapter;

namespace GroceryEcommerce.Infrastructure.Persistence.Repositories.Inventory;

public class PurchaseOrderRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<PurchaseOrderRepository> logger
) : BasePagedRepository<PurchaseOrderEntity, PurchaseOrder>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IPurchaseOrderRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("OrderNumber", typeof(string)),
            new SearchableField("SupplierId", typeof(Guid)),
            new SearchableField("WarehouseId", typeof(Guid)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("OrderDate", typeof(DateTime)),
            new SearchableField("ExpectedDate", typeof(DateTime)),
            new SearchableField("TotalAmount", typeof(decimal)),
            new SearchableField("CreatedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "OrderDate";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "OrderNumber", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "SupplierId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "WarehouseId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderDate", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "ExpectedDate", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TotalAmount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "OrderNumber", PurchaseOrderFields.OrderNumber },
            { "SupplierId", PurchaseOrderFields.SupplierId },
            { "WarehouseId", PurchaseOrderFields.WarehouseId },
            { "Status", PurchaseOrderFields.Status },
            { "OrderDate", PurchaseOrderFields.OrderDate },
            { "ExpectedDate", PurchaseOrderFields.ExpectedDate },
            { "TotalAmount", PurchaseOrderFields.TotalAmount },
            { "CreatedAt", PurchaseOrderFields.CreatedAt }
        };
    }

    protected override EntityQuery<PurchaseOrderEntity> ApplySearch(EntityQuery<PurchaseOrderEntity> query, string searchTerm)
    {
        return query.Where(PurchaseOrderFields.OrderNumber.Contains(searchTerm));
    }

    protected override EntityQuery<PurchaseOrderEntity> ApplySorting(EntityQuery<PurchaseOrderEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "ordernumber" => PurchaseOrderFields.OrderNumber,
            "orderdate" => PurchaseOrderFields.OrderDate,
            "expecteddate" => PurchaseOrderFields.ExpectedDate,
            "status" => PurchaseOrderFields.Status,
            "totalamount" => PurchaseOrderFields.TotalAmount,
            "createdat" => PurchaseOrderFields.CreatedAt,
            _ => PurchaseOrderFields.OrderDate
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<PurchaseOrderEntity> ApplyDefaultSorting(EntityQuery<PurchaseOrderEntity> query)
    {
        return query.OrderBy(PurchaseOrderFields.OrderDate.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return PurchaseOrderFields.PurchaseOrderId;
    }

    protected override object GetEntityId(PurchaseOrderEntity entity, EntityField2 primaryKeyField)
    {
        return entity.PurchaseOrderId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return PurchaseOrderFields.PurchaseOrderId.In(ids);
    }

    // Implementation of IPurchaseOrderRepository methods
    public async Task<Result<PurchaseOrder?>> GetByIdAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(PurchaseOrderFields.PurchaseOrderId, purchaseOrderId, "PurchaseOrder", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PurchaseOrder?>> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(PurchaseOrderFields.OrderNumber, orderNumber, "PurchaseOrder_Number", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PagedResult<PurchaseOrder>>> GetBySupplierIdAsync(Guid supplierId, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("SupplierId", supplierId),
            PurchaseOrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PurchaseOrder>> CreateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<PurchaseOrderEntity>(purchaseOrder);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            var domainEntity = Mapper.Map<PurchaseOrder>(entity);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrder created: {PurchaseOrderId}", entity.PurchaseOrderId);
            return Result<PurchaseOrder>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating purchase order");
            return Result<PurchaseOrder>.Failure("An error occurred while creating the purchase order.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(PurchaseOrder purchaseOrder, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<PurchaseOrderEntity>(purchaseOrder);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrder updated: {PurchaseOrderId}", entity.PurchaseOrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating purchase order: {PurchaseOrderId}", purchaseOrder.PurchaseOrderId);
            return Result<bool>.Failure("An error occurred while updating the purchase order.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new PurchaseOrderEntity(purchaseOrderId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrder deleted: {PurchaseOrderId}", purchaseOrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting purchase order: {PurchaseOrderId}", purchaseOrderId);
            return Result<bool>.Failure("An error occurred while deleting the purchase order.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid purchaseOrderId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(PurchaseOrderFields.PurchaseOrderId, purchaseOrderId, cancellationToken);
    }

    public async Task<Result<PagedResult<PurchaseOrder>>> GetByStatusAsync(short status, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("Status", status),
            PurchaseOrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<PurchaseOrder>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("OrderDate", fromDate, FilterOperator.GreaterThanOrEqual)
                      .WithFilter("OrderDate", toDate, FilterOperator.LessThanOrEqual),
            PurchaseOrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            // Get the last order number
            var query = qf.Create<PurchaseOrderEntity>()
                .OrderBy(PurchaseOrderFields.CreatedAt.Descending())
                .Limit(1);
            
            var lastOrder = await adapter.FetchFirstAsync(query, cancellationToken);
            
            var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{GenerateSequence(lastOrder?.OrderNumber)}";
            
            Logger.LogInformation("Generated purchase order number: {OrderNumber}", orderNumber);
            return Result<string>.Success(orderNumber);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating purchase order number");
            return Result<string>.Failure("An error occurred while generating order number.");
        }
    }

    private static string GenerateSequence(string? lastOrderNumber)
    {
        if (string.IsNullOrEmpty(lastOrderNumber))
            return "0001";
        
        var parts = lastOrderNumber.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var sequence))
        {
            // Check if it's from today
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            if (parts[1] == today)
            {
                return (sequence + 1).ToString("D4");
            }
        }
        
        return "0001";
    }

    public async Task<Result<bool>> UpdateStatusAsync(Guid purchaseOrderId, short status, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new PurchaseOrderEntity(purchaseOrderId) { IsNew = false };
            
            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("PurchaseOrder*", cancellationToken);
            
            Logger.LogInformation("PurchaseOrder status updated: {PurchaseOrderId}, Status: {Status}", purchaseOrderId, status);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating purchase order status: {PurchaseOrderId}", purchaseOrderId);
            return Result<bool>.Failure("An error occurred while updating purchase order status.");
        }
    }

    public async Task<Result<PagedResult<PurchaseOrder>>> GetPendingOrdersAsync(PagedRequest pagedRequest, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            pagedRequest,
            req => req.WithFilter("Status", new List<short> { 1, 2 }), 
            PurchaseOrderFields.OrderDate.Name,
            SortDirection.Ascending,
            cancellationToken
        );
    }

    public async Task<Result<decimal>> GetTotalValueBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<PurchaseOrderEntity>()
                .Where(PurchaseOrderFields.SupplierId == supplierId)
                .Select(() => PurchaseOrderFields.TotalAmount.Sum().As("TotalValue"));
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            
            Logger.LogInformation("Total purchase value for supplier {SupplierId}: {Total}", supplierId, total ?? 0);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting total value by supplier: {SupplierId}", supplierId);
            return Result<decimal>.Failure("An error occurred while getting total value.");
        }
    }
}

