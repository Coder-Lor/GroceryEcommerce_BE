using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.Orders.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Auth;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.Orders.Handlers;

public class CreateOrderHandler(
    IMapper mapper,
    ISalesRepository repository,
    IProductRepository productRepository,
    IProductVariantRepository productVariantRepository,
    IOrderPaymentRepository orderPaymentRepository,
    IUserRepository userRepository,
    ISepayService sepayService,
    IGiftCardRepository giftCardRepository,
    IConfiguration configuration,
    ILogger<CreateOrderHandler> logger
) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order for user: {UserId}", request.Request.UserId);

            // Validate and apply gift card if provided
            string? giftCardCode = null;
            decimal giftCardDiscountAmount = 0;
            
            if (!string.IsNullOrWhiteSpace(request.Request.CouponCode))
            {
                giftCardCode = request.Request.CouponCode;
                logger.LogInformation("Validating gift card code: {GiftCardCode}", giftCardCode);
                
                // Validate gift card
                var validateResult = await giftCardRepository.IsGiftCardValidAsync(giftCardCode, cancellationToken);
                if (!validateResult.IsSuccess || !validateResult.Data)
                {
                    return Result<OrderDto>.Failure($"Gift card code '{giftCardCode}' không hợp lệ hoặc không thể sử dụng.");
                }

                // Get gift card balance
                var balanceResult = await giftCardRepository.GetRemainingBalanceAsync(giftCardCode, cancellationToken);
                if (!balanceResult.IsSuccess)
                {
                    return Result<OrderDto>.Failure($"Không thể lấy số dư của gift card '{giftCardCode}'.");
                }

                var giftCardBalance = balanceResult.Data;
                
                // Calculate total amount before discount
                var totalBeforeDiscount = request.Request.SubTotal + request.Request.TaxAmount + request.Request.ShippingAmount;
                
                // Use gift card balance, but not more than total amount
                giftCardDiscountAmount = Math.Min(giftCardBalance, totalBeforeDiscount);
                
                logger.LogInformation("Gift card discount calculated: {DiscountAmount} (balance: {Balance}, total: {Total}) for gift card: {GiftCardCode}", 
                    giftCardDiscountAmount, giftCardBalance, totalBeforeDiscount, giftCardCode);
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
            
            // Apply gift card discount if any
            if (giftCardDiscountAmount > 0)
            {
                order.DiscountAmount = giftCardDiscountAmount;
                // Recalculate total amount with gift card discount
                order.TotalAmount = order.SubTotal + order.TaxAmount + order.ShippingAmount - order.DiscountAmount;
                // Ensure TotalAmount is not negative
                if (order.TotalAmount < 0)
                {
                    order.TotalAmount = 0;
                }
                logger.LogInformation("Applied gift card discount: {DiscountAmount}, New total: {TotalAmount}", 
                    giftCardDiscountAmount, order.TotalAmount);
            }

            // Create order items - Load ProductName from Product and ProductSku from ProductVariant
            if (request.Request.Items.Any())
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

            // Redeem gift card if gift card was applied
            if (!string.IsNullOrWhiteSpace(giftCardCode) && giftCardDiscountAmount > 0)
            {
                try
                {
                    logger.LogInformation("Redeeming gift card for order: {OrderId}, gift card code: {GiftCardCode}, amount: {Amount}", 
                        order.OrderId, giftCardCode, giftCardDiscountAmount);
                    
                    var redeemResult = await giftCardRepository.RedeemGiftCardAsync(giftCardCode, giftCardDiscountAmount, cancellationToken);
                    if (!redeemResult.IsSuccess)
                    {
                        logger.LogWarning("Failed to redeem gift card for order: {OrderId}, gift card code: {GiftCardCode}. Error: {Error}", 
                            order.OrderId, giftCardCode, redeemResult.ErrorMessage);
                        // Don't fail the order creation if gift card redemption fails
                    }
                    else
                    {
                        logger.LogInformation("Gift card redeemed successfully for order: {OrderId}, gift card code: {GiftCardCode}, amount: {Amount}", 
                            order.OrderId, giftCardCode, giftCardDiscountAmount);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail order creation
                    logger.LogError(ex, "Error redeeming gift card for order: {OrderId}, gift card code: {GiftCardCode}. Order was created successfully.", 
                        order.OrderId, giftCardCode);
                }
            }

            var response = mapper.Map<OrderDto>(orderResult.Data);

            // Nếu phương thức thanh toán là chuyển khoản (PaymentMethod = 3), tạo payment request với Sepay
            // Nhưng nếu TotalAmount = 0 (sau khi áp gift card), không cần gọi Sepay, đánh dấu đã thanh toán luôn
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

