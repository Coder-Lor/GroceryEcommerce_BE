using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class UpdateOrderItemHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    IProductRepository productRepository,
    IProductVariantRepository productVariantRepository,
    ILogger<UpdateOrderItemHandler> logger
) : IRequestHandler<UpdateOrderItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Updating order item: {OrderItemId}", request.OrderItemId);

            var orderItemResult = await repository.GetByIdAsync(request.OrderItemId, cancellationToken);
            if (!orderItemResult.IsSuccess || orderItemResult.Data is null)
            {
                return Result<bool>.Failure("Order item not found.");
            }

            var orderItem = orderItemResult.Data;
            
            // Validate ProductVariantId exists
            if (!orderItem.ProductVariantId.HasValue)
            {
                return Result<bool>.Failure("Order item must have a ProductVariantId.");
            }

            // Get product to populate product name
            var productResult = await productRepository.GetByIdAsync(orderItem.ProductId, cancellationToken);
            if (!productResult.IsSuccess || productResult.Data == null)
            {
                return Result<bool>.Failure($"Product not found: {orderItem.ProductId}");
            }

            // Get product variant to populate SKU and validate it exists
            var variantResult = await productVariantRepository.GetByIdAsync(orderItem.ProductVariantId.Value, cancellationToken);
            if (!variantResult.IsSuccess || variantResult.Data == null)
            {
                return Result<bool>.Failure($"Product variant not found: {orderItem.ProductVariantId.Value}");
            }

            var product = productResult.Data;
            var variant = variantResult.Data;

            // Validate variant belongs to the product
            if (variant.ProductId != orderItem.ProductId)
            {
                return Result<bool>.Failure($"Product variant {orderItem.ProductVariantId.Value} does not belong to product {orderItem.ProductId}");
            }
            
            // Update order item
            orderItem.UnitPrice = request.Request.UnitPrice;
            orderItem.Quantity = request.Request.Quantity;
            orderItem.TotalPrice = request.Request.UnitPrice * request.Request.Quantity;
            orderItem.ProductName = product.Name;  // Sync ProductName from Product
            orderItem.ProductSku = variant.Sku;    // Sync ProductSku from ProductVariant

            var updateResult = await repository.UpdateAsync(orderItem, cancellationToken);
            if (!updateResult.IsSuccess)
            {
                logger.LogError("Failed to update order item: {OrderItemId}", request.OrderItemId);
                return Result<bool>.Failure(updateResult.ErrorMessage ?? "Failed to update order item.");
            }

            logger.LogInformation("Order item updated successfully: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating order item: {OrderItemId}", request.OrderItemId);
            return Result<bool>.Failure("An error occurred while updating order item.");
        }
    }
}

