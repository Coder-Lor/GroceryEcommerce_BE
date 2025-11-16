using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class GetOrdersPagingHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrdersPagingHandler> logger
) : IRequestHandler<GetOrdersPagingQuery, Result<PagedResult<OrderDto>>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrdersPagingQuery - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

            var result = await repository.GetPagedAsync(request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderDto>>.Failure(result.ErrorMessage ?? "Failed to get paged orders.");
            }

            var mappedItems = mapper.Map<List<OrderDto>>(result.Data.Items);
            var response = new PagedResult<OrderDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged orders");
            return Result<PagedResult<OrderDto>>.Failure("An error occurred while retrieving orders.");
        }
    }
}

public class GetOrderByIdHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrderByIdHandler> logger
) : IRequestHandler<GetOrderByIdQuery, Result<OrderDetailDto>>
{
    public async Task<Result<OrderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrderByIdQuery for order: {OrderId}", request.OrderId);

            var orderResult = await repository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (!orderResult.IsSuccess || orderResult.Data is null)
            {
                logger.LogWarning("Order not found: {OrderId}", request.OrderId);
                return Result<OrderDetailDto>.Failure("Order not found");
            }

            var response = mapper.Map<OrderDetailDto>(orderResult.Data);
            logger.LogInformation("Order retrieved successfully: {OrderId}", request.OrderId);
            return Result<OrderDetailDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting order by ID: {OrderId}", request.OrderId);
            return Result<OrderDetailDto>.Failure("An error occurred while retrieving the order.");
        }
    }
}

public class GetOrdersByUserIdHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<GetOrdersByUserIdHandler> logger
) : IRequestHandler<GetOrdersByUserIdQuery, Result<PagedResult<OrderDto>>>
{
    public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrdersByUserIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Handling GetOrdersByUserIdQuery for user: {UserId}", request.UserId);

            var result = await repository.GetOrdersByUserIdAsync(request.UserId, request.Request, cancellationToken);
            if (!result.IsSuccess || result.Data is null)
            {
                return Result<PagedResult<OrderDto>>.Failure(result.ErrorMessage ?? "Failed to get orders.");
            }

            var mappedItems = mapper.Map<List<OrderDto>>(result.Data.Items);
            var response = new PagedResult<OrderDto>(
                mappedItems,
                result.Data.TotalCount,
                result.Data.Page,
                result.Data.PageSize
            );

            return Result<PagedResult<OrderDto>>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting orders by user ID: {UserId}", request.UserId);
            return Result<PagedResult<OrderDto>>.Failure("An error occurred while retrieving orders.");
        }
    }
}

public class CreateOrderHandler(
    IMapper mapper,
    ISalesRepository repository,
    IProductRepository productRepository,
    ILogger<CreateOrderHandler> logger
) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order for user: {UserId}", request.Request.UserId);

            // Generate order number
            var orderNumberResult = await repository.GenerateOrderNumberAsync(cancellationToken);
            if (!orderNumberResult.IsSuccess)
            {
                return Result<OrderDto>.Failure("Failed to generate order number.");
            }

            // Map request to domain entity
            var order = mapper.Map<Order>(request.Request);
            order.OrderId = Guid.NewGuid();
            order.OrderNumber = orderNumberResult.Data!;
            order.OrderDate = DateTime.UtcNow;
            order.CreatedAt = DateTime.UtcNow;

            // Create order items - Load ProductName and ProductSku from Product repository
            if (request.Request.Items != null && request.Request.Items.Any())
            {
                var orderItems = new List<OrderItem>();
                foreach (var item in request.Request.Items)
                {
                    var productResult = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (!productResult.IsSuccess || productResult.Data == null)
                    {
                        return Result<OrderDto>.Failure($"Product not found: {item.ProductId}");
                    }

                    var product = productResult.Data;
                    orderItems.Add(new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId,
                        ProductName = product.Name,
                        ProductSku = product.Sku,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.UnitPrice * item.Quantity
                    });
                }
                order.OrderItems = orderItems;
            }

            // Create order
            var createResult = await repository.CreateOrderAsync(order, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order");
                return Result<OrderDto>.Failure(createResult.ErrorMessage ?? "Failed to create order.");
            }

            // Get created order with details
            var orderResult = await repository.GetOrderByIdAsync(order.OrderId, cancellationToken);
            if (!orderResult.IsSuccess || orderResult.Data is null)
            {
                return Result<OrderDto>.Failure("Order created but could not be retrieved.");
            }

            var response = mapper.Map<OrderDto>(orderResult.Data);
            logger.LogInformation("Order created successfully: {OrderId}", order.OrderId);
            return Result<OrderDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order");
            return Result<OrderDto>.Failure("An error occurred while creating the order.");
        }
    }
}

public class UpdateOrderHandler(
    IMapper mapper,
    ISalesRepository repository,
    ILogger<UpdateOrderHandler> logger
) : IRequestHandler<UpdateOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order: {OrderId}", request.OrderId);

            var orderResult = await repository.GetOrderByIdAsync(request.OrderId, cancellationToken);
            if (!orderResult.IsSuccess || orderResult.Data is null)
            {
                return Result<bool>.Failure("Order not found.");
            }

            var order = orderResult.Data;
            order.Status = request.Request.Status;
            order.PaymentStatus = request.Request.PaymentStatus;
            order.Notes = request.Request.Notes;
            order.EstimatedDeliveryDate = request.Request.EstimatedDeliveryDate;
            order.UpdatedAt = DateTime.UtcNow;

            var updateResult = await repository.UpdateOrderAsync(order, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order: {OrderId}", request.OrderId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order.");
            }

            logger.LogInformation("Order updated successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while updating the order.");
        }
    }
}

public class DeleteOrderHandler(
    ISalesRepository repository,
    ILogger<DeleteOrderHandler> logger
) : IRequestHandler<DeleteOrderCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Deleting order: {OrderId}", request.OrderId);

            var result = await repository.DeleteOrderAsync(request.OrderId, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to delete order: {OrderId}", request.OrderId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete order.");
            }

            logger.LogInformation("Order deleted successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting order: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while deleting the order.");
        }
    }
}

public class UpdateOrderStatusHandler(
    ISalesRepository repository,
    ILogger<UpdateOrderStatusHandler> logger
) : IRequestHandler<UpdateOrderStatusCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order status: {OrderId}, Status: {Status}", request.OrderId, request.Status);

            var result = await repository.UpdateOrderStatusAsync(request.OrderId, request.Status, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to update order status: {OrderId}", request.OrderId);
                return Result<bool>.Failure(result.ErrorMessage ?? "Failed to update order status.");
            }

            logger.LogInformation("Order status updated successfully: {OrderId}", request.OrderId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order status: {OrderId}", request.OrderId);
            return Result<bool>.Failure("An error occurred while updating order status.");
        }
    }
}

