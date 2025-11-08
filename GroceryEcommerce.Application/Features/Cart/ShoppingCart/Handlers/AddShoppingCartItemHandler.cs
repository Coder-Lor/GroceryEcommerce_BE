using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Interfaces.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class AddShoppingCartItemHandler(
    ICartRepository cartRepository,
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

            var item = new Domain.Entities.Cart.ShoppingCartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart.CartId,
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitPrice = 0, // pricing set in repository/service layer
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
}


