using System.Linq;
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

public class OrderRepository(
    DataAccessAdapter scopedAdapter,
    IUnitOfWorkService unitOfWorkService,
    IMapper mapper,
    ICacheService cacheService,
    ILogger<OrderRepository> logger
) : BasePagedRepository<OrderEntity, Order>(scopedAdapter, unitOfWorkService, mapper, cacheService, logger), ISalesRepository
{
    public override IReadOnlyList<SearchableField> GetSearchableFields()
    {
        return new List<SearchableField>
        {
            new SearchableField("OrderNumber", typeof(string)),
            new SearchableField("OrderDate", typeof(DateTime)),
            new SearchableField("Status", typeof(short)),
            new SearchableField("PaymentStatus", typeof(short)),
            new SearchableField("TotalAmount", typeof(decimal)),
            new SearchableField("UserId", typeof(Guid)),
            new SearchableField("ShippingFirstName", typeof(string)),
            new SearchableField("ShippingLastName", typeof(string)),
            new SearchableField("ShippingEmail", typeof(string)),
            new SearchableField("BillingEmail", typeof(string)),
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
            new FieldMapping { FieldName = "OrderDate", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "Status", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "PaymentStatus", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "PaymentMethod", FieldType = typeof(short), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "TotalAmount", FieldType = typeof(decimal), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "UserId", FieldType = typeof(Guid), IsSearchable = false, IsSortable = true, IsFilterable = true },
            new FieldMapping { FieldName = "CreatedAt", FieldType = typeof(DateTime), IsSearchable = false, IsSortable = true, IsFilterable = true }
        };
    }

    protected override IReadOnlyDictionary<string, EntityField2> GetFieldMap()
    {
        return new Dictionary<string, EntityField2>(StringComparer.OrdinalIgnoreCase)
        {
            { "ordernumber", OrderFields.OrderNumber },
            { "orderdate", OrderFields.OrderDate },
            { "status", OrderFields.Status },
            { "paymentstatus", OrderFields.PaymentStatus },
            { "paymentmethod", OrderFields.PaymentMethod },
            { "totalamount", OrderFields.TotalAmount },
            { "userid", OrderFields.UserId },
            { "createdat", OrderFields.CreatedAt }
        };
    }

    protected override EntityQuery<OrderEntity> ApplySearch(EntityQuery<OrderEntity> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm)) return query;

        var predicate = SearchPredicateBuilder.BuildContainsPredicate(
            searchTerm,
            OrderFields.OrderNumber,
            OrderFields.ShippingFirstName,
            OrderFields.ShippingLastName,
            OrderFields.ShippingEmail,
            OrderFields.BillingEmail);

        return query.Where(predicate);
    }

    protected override EntityQuery<OrderEntity> ApplySorting(EntityQuery<OrderEntity> query, string? sortBy, SortDirection sortDirection)
    {
        var field = sortBy?.ToLower() switch
        {
            "ordernumber" => OrderFields.OrderNumber,
            "orderdate" => OrderFields.OrderDate,
            "status" => OrderFields.Status,
            "paymentstatus" => OrderFields.PaymentStatus,
            "paymentmethod" => OrderFields.PaymentMethod,
            "totalamount" => OrderFields.TotalAmount,
            "userid" => OrderFields.UserId,
            "createdat" => OrderFields.CreatedAt,
            _ => OrderFields.OrderDate
        };

        return sortDirection == SortDirection.Ascending
            ? query.OrderBy(field.Ascending())
            : query.OrderBy(field.Descending());
    }

    protected override EntityQuery<OrderEntity> ApplyDefaultSorting(EntityQuery<OrderEntity> query)
    {
        return query.OrderBy(OrderFields.OrderDate.Descending());
    }

    protected override EntityField2? GetPrimaryKeyField()
    {
        return OrderFields.OrderId;
    }

    protected override object GetEntityId(OrderEntity entity, EntityField2 primaryKeyField)
    {
        return entity.OrderId;
    }

    protected override IPredicate CreateIdFilter(EntityField2 primaryKeyField, List<object> ids)
    {
        return OrderFields.OrderId.In(ids);
    }

    // ISalesRepository implementation
    public async Task<Result<PagedResult<Order>>> GetOrdersByUserIdAsync(Guid userId, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("UserId", userId),
            OrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<Order?>> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderFields.OrderId, orderId, "Order", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<Order?>> GetOrderByNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await GetSingleAsync(OrderFields.OrderNumber, orderNumber, "Order_Number", TimeSpan.FromMinutes(15), cancellationToken);
    }

    public async Task<Result<PagedResult<Order>>> GetOrdersByStatusAsync(short status, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithFilter("Status", status),
            OrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<PagedResult<Order>>> GetOrdersByDateRangeAsync(DateTime fromDate, DateTime toDate, PagedRequest request, CancellationToken cancellationToken = default)
    {
        return await GetPagedConfiguredAsync(
            request,
            req => req.WithRangeFilter("OrderDate", fromDate, toDate),
            OrderFields.OrderDate.Name,
            SortDirection.Descending,
            cancellationToken
        );
    }

    public async Task<Result<bool>> CreateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderEntity>(order);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            
            // Save order items if they exist
            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var orderItemEntity = Mapper.Map<OrderItemEntity>(orderItem);
                    await adapter.SaveEntityAsync(orderItemEntity, cancellationToken: cancellationToken);
                }
                Logger.LogInformation("Created {Count} order items for order: {OrderId}", order.OrderItems.Count, entity.OrderId);
            }
            
            await CacheService.RemoveByPatternAsync("Order*", cancellationToken);
            
            Logger.LogInformation("Order created: {OrderId}", entity.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order");
            return Result<bool>.Failure("An error occurred while creating order.");
        }
    }

    public async Task<Result<bool>> UpdateOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderEntity>(order);
            entity.IsNew = false;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Order*", cancellationToken);
            
            Logger.LogInformation("Order updated: {OrderId}", entity.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order: {OrderId}", order.OrderId);
            return Result<bool>.Failure("An error occurred while updating order.");
        }
    }

    public async Task<Result<bool>> UpdateOrderStatusAsync(Guid orderId, short status, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderEntity(orderId) { IsNew = false };
            
            entity.Status = status;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Order*", cancellationToken);
            
            Logger.LogInformation("Order status updated: {OrderId}, Status: {Status}", orderId, status);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order status: {OrderId}", orderId);
            return Result<bool>.Failure("An error occurred while updating order status.");
        }
    }

    public async Task<Result<bool>> UpdateOrderPaymentStatusAsync(Guid orderId, short paymentStatus, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderEntity(orderId) { IsNew = false };
            
            entity.PaymentStatus = paymentStatus;
            entity.UpdatedAt = DateTime.UtcNow;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("Order*", cancellationToken);
            
            Logger.LogInformation("Order payment status updated: {OrderId}, PaymentStatus: {PaymentStatus}", orderId, paymentStatus);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order payment status: {OrderId}", orderId);
            return Result<bool>.Failure("An error occurred while updating order payment status.");
        }
    }

    public async Task<Result<bool>> DeleteOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderEntity(orderId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("Order*", cancellationToken);
            
            Logger.LogInformation("Order deleted: {OrderId}", orderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order: {OrderId}", orderId);
            return Result<bool>.Failure("An error occurred while deleting order.");
        }
    }

    public async Task<Result<string>> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<OrderEntity>()
                .OrderBy(OrderFields.CreatedAt.Descending())
                .Limit(1);
            
            var lastOrder = await adapter.FetchFirstAsync(query, cancellationToken);
            
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{GenerateSequence(lastOrder?.OrderNumber)}";
            
            Logger.LogInformation("Generated order number: {OrderNumber}", orderNumber);
            return Result<string>.Success(orderNumber);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error generating order number");
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
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            if (parts[1] == today)
            {
                return (sequence + 1).ToString("D4");
            }
        }
        
        return "0001";
    }

    // Order Item operations (delegated to IOrderItemRepository but kept for backward compatibility)
    public async Task<Result<List<OrderItem>>> GetOrderItemsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId == orderId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderItem>>(entities);
            
            return Result<List<OrderItem>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order items for order: {OrderId}", orderId);
            return Result<List<OrderItem>>.Failure("An error occurred while fetching order items.");
        }
    }

    public async Task<Result<OrderItem?>> GetOrderItemByIdAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderItemId == orderItemId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<OrderItem?>.Success(null);
            
            var domainEntity = Mapper.Map<OrderItem>(entity);
            return Result<OrderItem?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order item: {OrderItemId}", orderItemId);
            return Result<OrderItem?>.Failure("An error occurred while fetching order item.");
        }
    }

    public async Task<Result<bool>> CreateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderItemEntity>(orderItem);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item created: {OrderItemId}", entity.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order item");
            return Result<bool>.Failure("An error occurred while creating order item.");
        }
    }

    public async Task<Result<bool>> UpdateOrderItemAsync(OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderItemEntity>(orderItem);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item updated: {OrderItemId}", entity.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating order item: {OrderItemId}", orderItem.OrderItemId);
            return Result<bool>.Failure("An error occurred while updating order item.");
        }
    }

    public async Task<Result<bool>> DeleteOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new OrderItemEntity(orderItemId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderItem*", cancellationToken);
            
            Logger.LogInformation("Order item deleted: {OrderItemId}", orderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting order item: {OrderItemId}", orderItemId);
            return Result<bool>.Failure("An error occurred while deleting order item.");
        }
    }

    // Order Status History operations
    public async Task<Result<List<OrderStatusHistory>>> GetOrderStatusHistoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderStatusHistoryEntity>()
                .Where(OrderStatusHistoryFields.OrderId == orderId)
                .OrderBy(OrderStatusHistoryFields.CreatedAt.Descending());
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<OrderStatusHistory>>(entities);
            
            return Result<List<OrderStatusHistory>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order status history for order: {OrderId}", orderId);
            return Result<List<OrderStatusHistory>>.Failure("An error occurred while fetching order status history.");
        }
    }

    public async Task<Result<bool>> CreateOrderStatusHistoryAsync(OrderStatusHistory statusHistory, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderStatusHistoryEntity>(statusHistory);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("OrderStatusHistory*", cancellationToken);
            
            Logger.LogInformation("Order status history created: {HistoryId}", entity.HistoryId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating order status history");
            return Result<bool>.Failure("An error occurred while creating order status history.");
        }
    }

    // Order Payment operations (simplified - full implementation should be in OrderPaymentRepository)
    public async Task<Result<List<OrderPayment>>> GetOrderPaymentsAsync(Guid orderId, CancellationToken cancellationToken = default)
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

    public async Task<Result<OrderPayment?>> GetOrderPaymentByIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderPaymentEntity>()
                .Where(OrderPaymentFields.PaymentId == paymentId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<OrderPayment?>.Success(null);
            
            var domainEntity = Mapper.Map<OrderPayment>(entity);
            return Result<OrderPayment?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order payment: {PaymentId}", paymentId);
            return Result<OrderPayment?>.Failure("An error occurred while fetching order payment.");
        }
    }

    public async Task<Result<bool>> CreateOrderPaymentAsync(OrderPayment payment, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> UpdateOrderPaymentAsync(OrderPayment payment, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> DeleteOrderPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default)
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

    // Order Shipment operations
    public async Task<Result<List<OrderShipment>>> GetOrderShipmentsAsync(Guid orderId, CancellationToken cancellationToken = default)
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

    public async Task<Result<OrderShipment?>> GetOrderShipmentByIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderShipmentEntity>()
                .Where(OrderShipmentFields.ShipmentId == shipmentId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<OrderShipment?>.Success(null);
            
            var domainEntity = Mapper.Map<OrderShipment>(entity);
            return Result<OrderShipment?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order shipment: {ShipmentId}", shipmentId);
            return Result<OrderShipment?>.Failure("An error occurred while fetching order shipment.");
        }
    }

    public async Task<Result<bool>> CreateOrderShipmentAsync(OrderShipment shipment, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> UpdateOrderShipmentAsync(OrderShipment shipment, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<OrderShipmentEntity>(shipment);
            entity.IsNew = false;
            
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

    public async Task<Result<bool>> DeleteOrderShipmentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
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

    // Shipment Item operations
    public async Task<Result<List<ShipmentItem>>> GetShipmentItemsAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentItemEntity>()
                .Where(ShipmentItemFields.ShipmentId == shipmentId);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<ShipmentItem>>(entities);
            
            return Result<List<ShipmentItem>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching shipment items for shipment: {ShipmentId}", shipmentId);
            return Result<List<ShipmentItem>>.Failure("An error occurred while fetching shipment items.");
        }
    }

    public async Task<Result<ShipmentItem?>> GetShipmentItemByIdAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentItemEntity>()
                .Where(ShipmentItemFields.ShipmentItemId == shipmentItemId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<ShipmentItem?>.Success(null);
            
            var domainEntity = Mapper.Map<ShipmentItem>(entity);
            return Result<ShipmentItem?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching shipment item: {ShipmentItemId}", shipmentItemId);
            return Result<ShipmentItem?>.Failure("An error occurred while fetching shipment item.");
        }
    }

    public async Task<Result<bool>> CreateShipmentItemAsync(ShipmentItem shipmentItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentItemEntity>(shipmentItem);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentItem*", cancellationToken);
            
            Logger.LogInformation("Shipment item created: {ShipmentItemId}", entity.ShipmentItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating shipment item");
            return Result<bool>.Failure("An error occurred while creating shipment item.");
        }
    }

    public async Task<Result<bool>> UpdateShipmentItemAsync(ShipmentItem shipmentItem, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentItemEntity>(shipmentItem);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentItem*", cancellationToken);
            
            Logger.LogInformation("Shipment item updated: {ShipmentItemId}", entity.ShipmentItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating shipment item: {ShipmentItemId}", shipmentItem.ShipmentItemId);
            return Result<bool>.Failure("An error occurred while updating shipment item.");
        }
    }

    public async Task<Result<bool>> DeleteShipmentItemAsync(Guid shipmentItemId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShipmentItemEntity(shipmentItemId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentItem*", cancellationToken);
            
            Logger.LogInformation("Shipment item deleted: {ShipmentItemId}", shipmentItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting shipment item: {ShipmentItemId}", shipmentItemId);
            return Result<bool>.Failure("An error occurred while deleting shipment item.");
        }
    }

    // Shipment Carrier operations
    public async Task<Result<List<ShipmentCarrier>>> GetShipmentCarriersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentCarrierEntity>();
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            var domainEntities = Mapper.Map<List<ShipmentCarrier>>(entities);
            
            return Result<List<ShipmentCarrier>>.Success(domainEntities);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching shipment carriers");
            return Result<List<ShipmentCarrier>>.Failure("An error occurred while fetching shipment carriers.");
        }
    }

    public async Task<Result<ShipmentCarrier?>> GetShipmentCarrierByIdAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<ShipmentCarrierEntity>()
                .Where(ShipmentCarrierFields.CarrierId == carrierId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<ShipmentCarrier?>.Success(null);
            
            var domainEntity = Mapper.Map<ShipmentCarrier>(entity);
            return Result<ShipmentCarrier?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching shipment carrier: {CarrierId}", carrierId);
            return Result<ShipmentCarrier?>.Failure("An error occurred while fetching shipment carrier.");
        }
    }

    public async Task<Result<bool>> CreateShipmentCarrierAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentCarrierEntity>(carrier);
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier created: {CarrierId}", entity.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating shipment carrier");
            return Result<bool>.Failure("An error occurred while creating shipment carrier.");
        }
    }

    public async Task<Result<bool>> UpdateShipmentCarrierAsync(ShipmentCarrier carrier, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = Mapper.Map<ShipmentCarrierEntity>(carrier);
            entity.IsNew = false;
            
            await adapter.SaveEntityAsync(entity, cancellationToken: cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier updated: {CarrierId}", entity.CarrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating shipment carrier: {CarrierId}", carrier.CarrierId);
            return Result<bool>.Failure("An error occurred while updating shipment carrier.");
        }
    }

    public async Task<Result<bool>> DeleteShipmentCarrierAsync(Guid carrierId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var entity = new ShipmentCarrierEntity(carrierId) { IsNew = false };
            
            await adapter.DeleteEntityAsync(entity, cancellationToken);
            await CacheService.RemoveByPatternAsync("ShipmentCarrier*", cancellationToken);
            
            Logger.LogInformation("Shipment carrier deleted: {CarrierId}", carrierId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting shipment carrier: {CarrierId}", carrierId);
            return Result<bool>.Failure("An error occurred while deleting shipment carrier.");
        }
    }

    // Order Refund operations
    public async Task<Result<List<OrderRefund>>> GetOrderRefundsAsync(Guid orderId, CancellationToken cancellationToken = default)
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

    public async Task<Result<OrderRefund?>> GetOrderRefundByIdAsync(Guid refundId, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderRefundEntity>()
                .Where(OrderRefundFields.RefundId == refundId);
            
            var entity = await adapter.FetchFirstAsync(query, cancellationToken);
            if (entity == null)
                return Result<OrderRefund?>.Success(null);
            
            var domainEntity = Mapper.Map<OrderRefund>(entity);
            return Result<OrderRefund?>.Success(domainEntity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching order refund: {RefundId}", refundId);
            return Result<OrderRefund?>.Failure("An error occurred while fetching order refund.");
        }
    }

    public async Task<Result<bool>> CreateOrderRefundAsync(OrderRefund refund, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> UpdateOrderRefundAsync(OrderRefund refund, CancellationToken cancellationToken = default)
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

    public async Task<Result<bool>> DeleteOrderRefundAsync(Guid refundId, CancellationToken cancellationToken = default)
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

    // Sales analytics
    public async Task<Result<decimal>> GetTotalSalesByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderEntity>()
                .Where(OrderFields.OrderDate >= fromDate & OrderFields.OrderDate <= toDate)
                .Select(() => OrderFields.TotalAmount.Sum());
            
            var total = await adapter.FetchScalarAsync<decimal?>(query, cancellationToken);
            return Result<decimal>.Success(total ?? 0);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating total sales");
            return Result<decimal>.Failure("An error occurred while calculating total sales.");
        }
    }

    public async Task<Result<int>> GetOrderCountByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderEntity>()
                .Where(OrderFields.OrderDate >= fromDate & OrderFields.OrderDate <= toDate)
                .Select(() => Functions.CountRow());
            
            var count = await adapter.FetchScalarAsync<int>(query, cancellationToken);
            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating order count");
            return Result<int>.Failure("An error occurred while calculating order count.");
        }
    }

    public async Task<Result<decimal>> GetAverageOrderValueByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            var query = qf.Create<OrderEntity>()
                .Where(OrderFields.OrderDate >= fromDate & OrderFields.OrderDate <= toDate);
            
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            if (entities.Count == 0)
                return Result<decimal>.Success(0);
            
            var avg = entities.Cast<OrderEntity>().Average(e => e.TotalAmount);
            return Result<decimal>.Success(avg);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error calculating average order value");
            return Result<decimal>.Failure("An error occurred while calculating average order value.");
        }
    }

    public async Task<Result<List<object>>> GetTopSellingProductsAsync(int limit = 10, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var orderQuery = qf.Create<OrderEntity>();
            if (fromDate.HasValue && toDate.HasValue)
            {
                orderQuery = orderQuery.Where(OrderFields.OrderDate >= fromDate.Value & OrderFields.OrderDate <= toDate.Value);
            }
            
            var orderItemQuery = qf.Create<OrderItemEntity>()
                .Where(OrderItemFields.OrderId.In(
                    orderQuery.Select(OrderFields.OrderId)
                ));
            
            // Fetch all items and group in memory
            var entities = await adapter.FetchQueryAsync(orderItemQuery, cancellationToken);
            
            var grouped = entities.Cast<OrderItemEntity>()
                .GroupBy(e => e.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(e => e.Quantity),
                    TotalRevenue = g.Sum(e => e.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(limit)
                .Select(x => new
                {
                    x.ProductId,
                    x.TotalQuantity,
                    x.TotalRevenue
                } as object)
                .ToList();
            
            return Result<List<object>>.Success(grouped);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching top selling products");
            return Result<List<object>>.Failure("An error occurred while fetching top selling products.");
        }
    }

    public async Task<Result<List<object>>> GetSalesByStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var adapter = GetAdapter();
            var qf = new QueryFactory();
            
            var query = qf.Create<OrderEntity>();
            
            // Fetch all orders and group in memory
            var entities = await adapter.FetchQueryAsync(query, cancellationToken);
            
            var results = entities.Cast<OrderEntity>()
                .GroupBy(e => e.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    OrderCount = g.Count(),
                    TotalAmount = g.Sum(e => e.TotalAmount)
                })
                .Select(x => new
                {
                    x.Status,
                    x.OrderCount,
                    x.TotalAmount
                } as object)
                .ToList();
            
            return Result<List<object>>.Success(results);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error fetching sales by status");
            return Result<List<object>>.Failure("An error occurred while fetching sales by status.");
        }
    }
}

