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

public class OrderRefundRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderRefundRepository> logger
) : BasePagedRepository<OrderRefundEntity, OrderRefund>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IOrderRefundRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("PaymentId", typeof(Guid)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("Amount", typeof(decimal)),
            new SearchableField("Reason", typeof(string)),
            new SearchableField("RequestedAt", typeof(DateTime))
        };
    }

    public override string GetDefaultSortField()
    {
        return "RequestedAt";
    }

    public override IReadOnlyList<FieldMapping> GetFieldMappings()
    {
        return new List<FieldMapping>
        {
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "PaymentId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Amount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Reason", FieldType = typeof(string), IsSearchable = true, IsSortable = false, IsFilterable = true },
            new FieldMapping { FieldName = "RequestedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "orderid", OrderRefundFields.OrderId },
            { "paymentid", OrderRefundFields.PaymentId },
            { "status", OrderRefundFields.Status },
            { "amount", OrderRefundFields.Amount },
            { "reason", OrderRefundFields.Reason },
            { "requestedat", OrderRefundFields.RequestedAt }
        };
    }

    protected override EntityQuery<OrderRefundEntity> ApplySearch(EntityQuery<OrderRefundEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        return query.Where(SearchPredicateBuilder.BuildContainsPredicate(searchTerm, OrderRefundFields.Reason));
    }

    protected override EntityQuery<OrderRefundEntity> ApplySorting(EntityQuery<OrderRefundEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "orderid" => OrderRefundFields.OrderId,
            "paymentid" => OrderRefundFields.PaymentId,
            "status" => OrderRefundFields.Status,
            "amount" => OrderRefundFields.Amount,
            "requestedat" => OrderRefundFields.RequestedAt,
            _ => OrderRefundFields.RequestedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderRefundEntity> ApplyDefaultSorting(EntityQuery<OrderRefundEntity> query)
    {
        return query.OrderBy(OrderRefundFields.RequestedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderRefundFields.RefundId;
    }

    protected override object GetEntityId(OrderRefundEntity entity, EntityField2 primaryKeyField)
    {
        return entity.RefundId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderRefundFields.RefundId.In(ids);
    }

    public async Task<Result<OrderRefund?>> GetByIdAsync(Guid refundId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderRefundFields.RefundId, refundId, "OrderRefund", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<OrderRefund>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.OrderId == orderId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderRefund>>(entities);
            
            return Result<List<OrderRefund>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order refunds for order: {OrderId}", orderId);
            return Result<List<OrderRefund>>.Failure("An error occurred while fetching order refunds.");
        }
    }

    public async Task<Result<List<OrderRefund>>> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.PaymentId == paymentId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderRefund>>(entities);
            
            return Result<List<OrderRefund>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order refunds for payment: {PaymentId}", paymentId);
            return Result<List<OrderRefund>>.Failure("An error occurred while fetching order refunds.");
        }
    }

    public async Task<Result<bool>> CreateAsync(OrderRefund refund, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderRefundEntity>(refund);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderRefund*", cancellationToken);
            
            Logger.LogInformation("Order refund created: {RefundId}", entity.RefundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order refund");
            return Result<bool>.Failure("An error occurred while creating order refund.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(OrderRefund refund, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderRefundEntity>(refund);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderRefund*", cancellationToken);
            
            Logger.LogInformation("Order refund updated: {RefundId}", entity.RefundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order refund: {RefundId}", refund.RefundId);
            return Result<bool>.Failure("An error occurred while updating order refund.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid refundId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderRefundEntity(refundId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderRefund*", cancellationToken);
            
            Logger.LogInformation("Order refund deleted: {RefundId}", refundId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order refund: {RefundId}", refundId);
            return Result<bool>.Failure("An error occurred while deleting order refund.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid refundId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(OrderRefundFields.RefundId, refundId, cancellationToken);
    }

    public async Task<Result<PagedResult<OrderRefund>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", status),
            OrderRefundFields.RequestedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderRefund>>> GetPendingRefundsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", 1), // Status 1 = Requested
            OrderRefundFields.RequestedAt.Name,
            SortDirection.Ascending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderRefund>>> GetProcessedRefundsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", 2), // Status 2 = Processed
            OrderRefundFields.RequestedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<decimal>> GetTotalRefundedByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.OrderId == orderId & OrderRefundFields.Status == 2) // Status 2 = Processed
                .Select(OrderRefundFields.Amount.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating total refunded for order: {OrderId}", orderId);
            return Result<decimal>.Failure("An error occurred while calculating total refunded.");
        }
    }

    public async Task<Result<bool>> ProcessRefundAsync(Guid refundId, Guid processedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.RefundId == refundId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                return Result<bool>.Failure("Refund not found.");
            }
            
            entity.Status = 2; // Processed
            entity.ProcessedAt = DateTime.UtcNow;
            entity.ProcessedBy = processedBy;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderRefund*", cancellationToken);
            
            Logger.LogInformation("Order refund processed: {RefundId}, ProcessedBy: {ProcessedBy}", refundId, processedBy);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing refund: {RefundId}", refundId);
            return Result<bool>.Failure("An error occurred while processing refund.");
        }
    }

    public async Task<Result<int>> GetPendingRefundCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.Status == 1) // Status 1 = Requested
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting pending refunds");
            return Result<int>.Failure("An error occurred while counting pending refunds.");
        }
    }
}

