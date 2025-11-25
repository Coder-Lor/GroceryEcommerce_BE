using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Features.Sales.Orders.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

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
    IProductVariantRepository productVariantRepository,
    IOrderPaymentRepository orderPaymentRepository,
    IUserRepository userRepository,
    ISepayService sepayService,
    IConfiguration configuration,
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

            // Create order items - Load ProductName from Product and ProductSku from ProductVariant
            if (request.Request.Items != null && request.Request.Items.Any())
            {
                var orderItems = new List<OrderItem>();
                foreach (var item in request.Request.Items)
                {
                    // Validate ProductVariantId is required
                    if (!item.ProductVariantId.HasValue)
                    {
                        return Result<OrderDto>.Failure($"ProductVariantId is required for order item with ProductId: {item.ProductId}");
                    }

                    // Get product to populate product name
                    var productResult = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (!productResult.IsSuccess || productResult.Data == null)
                    {
                        return Result<OrderDto>.Failure($"Product not found: {item.ProductId}");
                    }

                    // Get product variant to populate SKU and validate it exists
                    var variantResult = await productVariantRepository.GetByIdAsync(item.ProductVariantId.Value, cancellationToken);
                    if (!variantResult.IsSuccess || variantResult.Data == null)
                    {
                        return Result<OrderDto>.Failure($"Product variant not found: {item.ProductVariantId.Value}");
                    }

                    var product = productResult.Data;
                    var variant = variantResult.Data;

                    // Validate variant belongs to the product
                    if (variant.ProductId != item.ProductId)
                    {
                        return Result<OrderDto>.Failure($"Product variant {item.ProductVariantId.Value} does not belong to product {item.ProductId}");
                    }

                    orderItems.Add(new OrderItem
                    {
                        OrderItemId = Guid.NewGuid(),
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        ProductVariantId = item.ProductVariantId.Value,
                        ProductName = product.Name,  // Use product name
                        ProductSku = variant.Sku,    // Use variant SKU
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

            // Nếu phương thức thanh toán là chuyển khoản (PaymentMethod = 3), tạo payment request với Sepay
            if (order.PaymentMethod == 3) // 3 = Bank Transfer
            {
                try
                {
                    logger.LogInformation("Creating payment request for bank transfer order: {OrderId}", order.OrderId);

                    // Lấy thông tin user
                    var userResult = await userRepository.GetByIdAsync(order.UserId, cancellationToken);
                    if (!userResult.IsSuccess || userResult.Data == null)
                    {
                        logger.LogWarning("User not found for order: {OrderId}, but order was created. Payment request will not be created.", order.OrderId);
                    }
                    else
                    {
                        var user = userResult.Data;
                        var customerName = $"{user.FirstName} {user.LastName}".Trim();
                        if (string.IsNullOrEmpty(customerName))
                        {
                            customerName = user.Username;
                        }

                        // Tạo payment request với Sepay
                        var paymentRequest = new CreateSepayPaymentRequest
                        {
                            OrderId = order.OrderId,
                            OrderNumber = order.OrderNumber,
                            Amount = order.TotalAmount,
                            Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                            CustomerName = customerName,
                            CustomerEmail = user.Email,
                            CustomerPhone = user.PhoneNumber,
                            ReturnUrl = $"{configuration["BaseUrl"] ?? "https://localhost"}/order/{order.OrderId}/success",
                            CancelUrl = $"{configuration["BaseUrl"] ?? "https://localhost"}/order/{order.OrderId}/cancel"
                        };

                        var paymentResult = await sepayService.CreatePaymentAsync(paymentRequest);
                        if (paymentResult.IsSuccess && paymentResult.Data != null)
                        {
                            // Tạo OrderPayment record
                            var orderPayment = new OrderPayment
                            {
                                PaymentId = Guid.NewGuid(),
                                OrderId = order.OrderId,
                                PaymentMethod = 3, // Bank Transfer
                                Amount = order.TotalAmount,
                                TransactionId = paymentResult.Data.TransactionId,
                                GatewayResponse = System.Text.Json.JsonSerializer.Serialize(paymentResult.Data),
                                Status = 1, // Pending
                                Currency = "VND",
                                CreatedAt = DateTime.UtcNow
                            };

                            var createPaymentResult = await orderPaymentRepository.CreateAsync(orderPayment, cancellationToken);
                            if (createPaymentResult.IsSuccess)
                            {
                                // Thêm thông tin payment vào response
                                response.PaymentUrl = paymentResult.Data.PaymentUrl;
                                response.QrCodeUrl = paymentResult.Data.QrCodeUrl;
                                response.PaymentTransactionId = paymentResult.Data.TransactionId;
                                logger.LogInformation("Payment request created successfully for order: {OrderId}, TransactionId: {TransactionId}", 
                                    order.OrderId, paymentResult.Data.TransactionId);
                            }
                            else
                            {
                                logger.LogWarning("Order payment record creation failed for order: {OrderId}. Error: {Error}", 
                                    order.OrderId, createPaymentResult.ErrorMessage);
                            }
                        }
                        else
                        {
                            // Tạo payment record với status failed nếu payment creation fail
                            var failedPayment = new OrderPayment
                            {
                                PaymentId = Guid.NewGuid(),
                                OrderId = order.OrderId,
                                PaymentMethod = 3, // Bank Transfer
                                Amount = order.TotalAmount,
                                TransactionId = null,
                                GatewayResponse = paymentResult.ErrorMessage,
                                Status = 3, // Failed
                                Currency = "VND",
                                CreatedAt = DateTime.UtcNow
                            };
                            
                            await orderPaymentRepository.CreateAsync(failedPayment, cancellationToken);
                            logger.LogWarning("Sepay payment request creation failed for order: {OrderId}, Error: {Error}. Order payment record created with failed status.", 
                                order.OrderId, paymentResult.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi nhưng không fail order creation
                    logger.LogError(ex, "Error creating payment request for order: {OrderId}. Order was created successfully but payment request failed.", order.OrderId);
                }
            }

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

