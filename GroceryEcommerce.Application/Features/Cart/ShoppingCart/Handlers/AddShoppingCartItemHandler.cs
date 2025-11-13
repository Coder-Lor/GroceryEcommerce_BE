using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class AddShoppingCartItemHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IProductVariantRepository productVariantRepository,
    ICurrentUserService currentUserService,
    ILogger<AddShoppingCartItemHandler> logger
) : IRequestHandler<AddShoppingCartItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddShoppingCartItemCommand request, CancellationToken cancellationToken)
    {
       try {

            var userId = currentUserService.GetCurrentUserId() ?? Guid.Empty;
            logger.LogInformation("Adding item to cart for user {UserId}, product {ProductId}", userId, request.ProductId);
            
            // Ensure cart exists
            var cartResult = await cartRepository.GetShoppingCartByUserIdAsync(userId, cancellationToken);
            Domain.Entities.Cart.ShoppingCart cart;
            if (!cartResult.IsSuccess || cartResult.Data is null)
            {
                cart = new Domain.Entities.Cart.ShoppingCart
                {
                    CartId = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                var createCart = await cartRepository.CreateShoppingCartAsync(cart, cancellationToken);
                if (!createCart.IsSuccess)
                {
                    return Result<bool>.Failure(createCart.ErrorMessage ?? "Failed to create shopping cart");
                }
            }
            else
            {
                cart = cartResult.Data;
            }

            // Check existing item
            var existingItem = await cartRepository.GetShoppingCartItemByProductAsync(cart.CartId, request.ProductId, request.ProductVariantId, cancellationToken);
            if (existingItem.IsSuccess && existingItem.Data is not null)
            {
                var updateQty = await cartRepository.UpdateShoppingCartItemQuantityAsync(existingItem.Data.CartItemId, existingItem.Data.Quantity + request.Quantity, cancellationToken);
                return updateQty.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(updateQty.ErrorMessage ?? "Failed to update quantity");
            }

            var pricingResult = await ResolveUnitPriceAsync(request, cancellationToken);
            if (!pricingResult.IsSuccess)
            {
                return Result<bool>.Failure(pricingResult.ErrorMessage ?? "Unable to resolve product price");
            }

            if (pricingResult.Data is null || pricingResult.Data <= 0)
            {
                return Result<bool>.Failure("Product price must be greater than zero");
            }

            var item = new Domain.Entities.Cart.ShoppingCartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart.CartId,
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitPrice = pricingResult.Data.Value,
                CreatedAt = DateTime.UtcNow
            };

            var addRes = await cartRepository.AddShoppingCartItemAsync(item, cancellationToken);
            return addRes.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(addRes.ErrorMessage ?? "Failed to add item");
       }
       catch (Exception ex)
       {
        logger.LogError(ex, "Error adding shopping cart item");
        return Result<bool>.Failure(ex.Message);
       }
    }

    private async Task<Result<decimal?>> ResolveUnitPriceAsync(AddShoppingCartItemCommand request, CancellationToken cancellationToken)
    {
        if (request.ProductVariantId.HasValue)
        {
            var variantResult = await productVariantRepository.GetByIdAsync(request.ProductVariantId.Value, cancellationToken);
            if (!variantResult.IsSuccess || variantResult.Data is null)
            {
                return Result<decimal?>.Failure("Product variant not found");
            }

            if (variantResult.Data.ProductId != request.ProductId)
            {
                return Result<decimal?>.Failure("Product variant does not belong to the specified product");
            }

            var variantPrice = variantResult.Data.DiscountPrice ?? variantResult.Data.Price;
            return Result<decimal?>.Success(variantPrice);
        }

        var productResult = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess || productResult.Data is null)
        {
            return Result<decimal?>.Failure("Product not found");
        }

        var price = productResult.Data.DiscountPrice ?? productResult.Data.Price;
        return Result<decimal?>.Success(price);
    }
}


