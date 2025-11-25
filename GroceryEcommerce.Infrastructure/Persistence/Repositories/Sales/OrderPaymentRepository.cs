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

public class OrderPaymentRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderPaymentRepository> logger
) : BasePagedRepository<OrderPaymentEntity, OrderPayment>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), IOrderPaymentRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("TransactionId", typeof(string)),
            new SearchableField("OrderId", typeof(Guid)),
            new SearchableField("PaymentMethod", typeof(short)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("Amount", typeof(decimal)),
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
            new FieldMapping { FieldName = "TransactionId", FieldType = typeof(string), IsSearchable = true, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "OrderId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "PaymentMethod", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Amount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "transactionid", OrderPaymentFields.TransactionId },
            { "orderid", OrderPaymentFields.OrderId },
            { "paymentmethod", OrderPaymentFields.PaymentMethod },
            { "status", OrderPaymentFields.Status },
            { "amount", OrderPaymentFields.Amount },
            { "createdat", OrderPaymentFields.CreatedAt }
        };
    }

    protected override EntityQuery<OrderPaymentEntity> ApplySearch(EntityQuery<OrderPaymentEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        return query.Where(SearchPredicateBuilder.BuildContainsPredicate(searchTerm, OrderPaymentFields.TransactionId));
    }

    protected override EntityQuery<OrderPaymentEntity> ApplySorting(EntityQuery<OrderPaymentEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "transactionid" => OrderPaymentFields.TransactionId,
            "orderid" => OrderPaymentFields.OrderId,
            "paymentmethod" => OrderPaymentFields.PaymentMethod,
            "status" => OrderPaymentFields.Status,
            "amount" => OrderPaymentFields.Amount,
            "createdat" => OrderPaymentFields.CreatedAt,
            _ => OrderPaymentFields.CreatedAt
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderPaymentEntity> ApplyDefaultSorting(EntityQuery<OrderPaymentEntity> query)
    {
        return query.OrderBy(OrderPaymentFields.CreatedAt.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderPaymentFields.PaymentId;
    }

    protected override object GetEntityId(OrderPaymentEntity entity, EntityField2 primaryKeyField)
    {
        return entity.PaymentId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderPaymentFields.PaymentId.In(ids);
    }

    public async Task<Result<OrderPayment?>> GetByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderPaymentFields.PaymentId, paymentId, "OrderPayment", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<List<OrderPayment>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderPaymentEntity>()
                .Where(OrderPaymentFields.OrderId == orderId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderPayment>>(entities);
            
            return Result<List<OrderPayment>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order payments for order: {OrderId}", orderId);
            return Result<List<OrderPayment>>.Failure("An error occurred while fetching order payments.");
        }
    }

    public async Task<Result<OrderPayment?>> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderPaymentFields.TransactionId, transactionId, "OrderPayment_Transaction", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<bool>> CreateAsync(OrderPayment payment, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderPaymentEntity>(payment);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderPayment*", cancellationToken);
            
            Logger.LogInformation("Order payment created: {PaymentId}", entity.PaymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order payment");
            return Result<bool>.Failure("An error occurred while creating order payment.");
        }
    }

    public async Task<Result<bool>> UpdateAsync(OrderPayment payment, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderPaymentEntity>(payment);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderPayment*", cancellationToken);
            
            Logger.LogInformation("Order payment updated: {PaymentId}", entity.PaymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order payment: {PaymentId}", payment.PaymentId);
            return Result<bool>.Failure("An error occurred while updating order payment.");
        }
    }

    public async Task<Result<bool>> DeleteAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderPaymentEntity(paymentId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderPayment*", cancellationToken);
            
            Logger.LogInformation("Order payment deleted: {PaymentId}", paymentId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order payment: {PaymentId}", paymentId);
            return Result<bool>.Failure("An error occurred while deleting order payment.");
        }
    }

    public async Task<Result<bool>> ExistsAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        return await ExistsByCountAsync(OrderPaymentFields.PaymentId, paymentId, cancellationToken);
    }

    public async Task<Result<PagedResult<OrderPayment>>> GetByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", status),
            OrderPaymentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderPayment>>> GetByPaymentMethodAsync(short paymentMethod, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("PaymentMethod", paymentMethod),
            OrderPaymentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<OrderPayment>>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithRangeFilter("CreatedAt", fromDate, toDate),
            OrderPaymentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<decimal>> GetTotalPaidByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderPaymentEntity>()
                .Where(OrderPaymentFields.OrderId == orderId & OrderPaymentFields.Status == 2) // Status 2 = Paid
                .Select(() => OrderPaymentFields.Amount.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating total paid for order: {OrderId}", orderId);
            return Result<decimal>.Failure("An error occurred while calculating total paid.");
        }
    }

    public async Task<Result<bool>> UpdatePaymentStatusAsync(Guid paymentId, short status, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderPaymentEntity>()
                .Where(OrderPaymentFields.PaymentId == paymentId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
            {
                return Result<bool>.Failure("Payment not found.");
            }
            
            entity.Status = status;
            if (status == 2) // Paid
            {
                entity.PaidAt = DateTime.UtcNow;
            }
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderPayment*", cancellationToken);
            
            Logger.LogInformation("Order payment status updated: {PaymentId}, Status: {Status}", paymentId, status);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating payment status: {PaymentId}", paymentId);
            return Result<bool>.Failure("An error occurred while updating payment status.");
        }
    }

    public async Task<Result<PagedResult<OrderPayment>>> GetFailedPaymentsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", 3), // Status 3 = Failed
            OrderPaymentFields.CreatedAt.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }
}

