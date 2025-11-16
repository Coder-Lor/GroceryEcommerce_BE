using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Sales.OrderItems.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Repositories.Sales;
using GroceryEcommerce.Application.Models.Sales;
using GroceryEcommerce.Domain.Entities.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Sales.OrderItems.Handlers;

public class CreateOrderItemHandler(
    IMapper mapper,
    IOrderItemRepository repository,
    IProductRepository productRepository,
    ILogger<CreateOrderItemHandler> logger
) : IRequestHandler<CreateOrderItemCommand, Result<OrderItemDto>>
{
    public async Task<Result<OrderItemDto>> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating order item for order: {OrderId}", request.Request.OrderId);

            // Get product to populate name and SKU
            var productResult = await productRepository.GetByIdAsync(request.Request.ProductId, cancellationToken);
            if (!productResult.IsSuccess || productResult.Data == null)
            {
                return Result<OrderItemDto>.Failure($"Product not found: {request.Request.ProductId}");
            }

            var product = productResult.Data;
            var orderItem = new OrderItem
            {
                OrderItemId = Guid.NewGuid(),
                OrderId = request.Request.OrderId,
                ProductId = request.Request.ProductId,
                ProductVariantId = request.Request.ProductVariantId,
                ProductName = product.Name,
                ProductSku = product.Sku,
                UnitPrice = request.Request.UnitPrice,
                Quantity = request.Request.Quantity,
                TotalPrice = request.Request.UnitPrice * request.Request.Quantity
            };

            var createResult = await repository.CreateAsync(orderItem, cancellationToken);
            if (!createResult.IsSuccess)
            {
                logger.LogError("Failed to create order item");
                return Result<OrderItemDto>.Failure(createResult.ErrorMessage ?? "Failed to create order item.");
            }

            var getResult = await repository.GetByIdAsync(orderItem.OrderItemId, cancellationToken);
            if (!getResult.IsSuccess || getResult.Data is null)
            {
                return Result<OrderItemDto>.Failure("Order item created but could not be retrieved.");
            }

            var response = mapper.Map<OrderItemDto>(getResult.Data);
            logger.LogInformation("Order item created successfully: {OrderItemId}", orderItem.OrderItemId);
            return Result<OrderItemDto>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating order item");
            return Result<OrderItemDto>.Failure("An error occurred while creating order item.");
        }
    }
}

