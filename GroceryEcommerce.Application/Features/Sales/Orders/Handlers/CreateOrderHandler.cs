using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Marketing;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class CreateOrderHandler(
    IMapper mapper,
    ISalesRepository repository,
    IProductRepository productRepository,
    IProductVariantRepository productVariantRepository,
    IOrderPaymentRepository orderPaymentRepository,
    IUserRepository userRepository,
    ISepayService sepayService,
    IMarketingRepository marketingRepository,
    ICouponUsageRepository couponUsageRepository,
    IConfiguration configuration,
    ILogger<CreateOrderHandler> logger
) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order for user: {UserId}", request.Request.UserId);

            // Validate and apply coupon if provided
            Guid? couponId = null;
            decimal couponDiscountAmount = 0;
            
            if (!string.IsNullOrWhiteSpace(request.Request.CouponCode))
            {
                logger.LogInformation("Validating coupon code: {CouponCode}", request.Request.CouponCode);
                
                // Get coupon by code
                var couponResult = await marketingRepository.GetCouponByCodeAsync(request.Request.CouponCode, cancellationToken);
                if (!couponResult.IsSuccess || couponResult.Data == null)
                {
                    return Result<OrderDto>.Failure($"Coupon code '{request.Request.CouponCode}' không tồn tại.");
                }

                var coupon = couponResult.Data;
                couponId = coupon.CouponId;

                // Validate coupon
                var validateResult = await marketingRepository.ValidateCouponAsync(
                    request.Request.CouponCode, 
                    request.Request.SubTotal, 
                    request.Request.UserId, 
                    cancellationToken);
                
                if (!validateResult.IsSuccess || !validateResult.Data)
                {
                    return Result<OrderDto>.Failure($"Coupon code '{request.Request.CouponCode}' không hợp lệ hoặc không thể sử dụng.");
                }

                // Calculate discount from coupon
                var discountResult = await marketingRepository.CalculateCouponDiscountAsync(
                    request.Request.CouponCode, 
                    request.Request.SubTotal, 
                    cancellationToken);
                
                if (!discountResult.IsSuccess)
                {
                    return Result<OrderDto>.Failure($"Không thể tính toán discount từ coupon code '{request.Request.CouponCode}'.");
                }

                couponDiscountAmount = discountResult.Data;
                logger.LogInformation("Coupon discount calculated: {DiscountAmount} for coupon: {CouponCode}", 
                    couponDiscountAmount, request.Request.CouponCode);
            }

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
            
            // Apply coupon discount if any
            if (couponDiscountAmount > 0)
            {
                order.DiscountAmount = couponDiscountAmount;
                // Recalculate total amount with coupon discount
                order.TotalAmount = order.SubTotal + order.TaxAmount + order.ShippingAmount - order.DiscountAmount;
                // Ensure TotalAmount is not negative
                if (order.TotalAmount < 0)
                {
                    order.TotalAmount = 0;
                }
                logger.LogInformation("Applied coupon discount: {DiscountAmount}, New total: {TotalAmount}", 
                    couponDiscountAmount, order.TotalAmount);
            }

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

            // Create CouponUsage record if coupon was applied
            if (couponId.HasValue && couponDiscountAmount > 0)
            {
                try
                {
                    logger.LogInformation("Creating CouponUsage record for order: {OrderId}, coupon: {CouponId}", 
                        order.OrderId, couponId.Value);
                    
                    var couponUsage = new CouponUsage
                    {
                        UsageId = Guid.NewGuid(),
                        CouponId = couponId.Value,
                        UserId = order.UserId,
                        OrderId = order.OrderId,
                        DiscountAmount = couponDiscountAmount,
                        UsedAt = DateTime.UtcNow
                    };

                    var couponUsageResult = await couponUsageRepository.CreateAsync(couponUsage, cancellationToken);
                    if (!couponUsageResult.IsSuccess)
                    {
                        logger.LogWarning("Failed to create CouponUsage record for order: {OrderId}, coupon: {CouponId}. Error: {Error}", 
                            order.OrderId, couponId.Value, couponUsageResult.ErrorMessage);
                        // Don't fail the order creation if coupon usage record creation fails
                    }
                    else
                    {
                        logger.LogInformation("CouponUsage record created successfully for order: {OrderId}", order.OrderId);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail order creation
                    logger.LogError(ex, "Error creating CouponUsage record for order: {OrderId}. Order was created successfully.", order.OrderId);
                }
            }

            var response = mapper.Map<OrderDto>(orderResult.Data);

            // Nếu phương thức thanh toán là chuyển khoản (PaymentMethod = 3), tạo payment request với Sepay
            // Nhưng nếu TotalAmount = 0 (sau khi áp mã giảm giá), không cần gọi Sepay, đánh dấu đã thanh toán luôn
            if (order.PaymentMethod == 3) // 3 = Bank Transfer
            {
                // Nếu tổng tiền = 0, đánh dấu đã thanh toán luôn, không cần gọi Sepay
                if (order.TotalAmount == 0)
                {
                    try
                    {
                        logger.LogInformation("Order total amount is 0, marking payment as completed without calling Sepay for order: {OrderId}", order.OrderId);
                        
                        var freeOrderPayment = new OrderPayment
                        {
                            PaymentId = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            PaymentMethod = 3, // Bank Transfer
                            Amount = 0,
                            TransactionId = order.OrderNumber,
                            GatewayResponse = "Order total is 0 after discount, payment automatically completed",
                            Status = 2, // Completed
                            Currency = "VND",
                            PaidAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        };

                        var createPaymentResult = await orderPaymentRepository.CreateAsync(freeOrderPayment, cancellationToken);
                        if (createPaymentResult.IsSuccess)
                        {
                            logger.LogInformation("Free order payment record created successfully for order: {OrderId}", order.OrderId);
                        }
                        else
                        {
                            logger.LogWarning("Failed to create free order payment record for order: {OrderId}. Error: {Error}", 
                                order.OrderId, createPaymentResult.ErrorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error creating free order payment record for order: {OrderId}", order.OrderId);
                    }
                }
                else
                {
                    // Tổng tiền > 0, gọi Sepay như bình thường
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
                                    TransactionId = order.OrderNumber,
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
                                    response.PaymentTransactionId = order.OrderNumber;
                                    logger.LogInformation("Payment request created successfully for order: {OrderId}, TransactionCode: {TransactionCode}", 
                                        order.OrderId, order.OrderNumber);
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

