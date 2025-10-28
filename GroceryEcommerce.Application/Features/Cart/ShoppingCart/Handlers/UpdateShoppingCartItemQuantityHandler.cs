using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class UpdateShoppingCartItemQuantityHandler(
    ICartRepository cartRepository,
    ILogger<UpdateShoppingCartItemQuantityHandler> logger
) : IRequestHandler<UpdateShoppingCartItemQuantityCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateShoppingCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating cart item {CartItemId} quantity to {Quantity}", request.CartItemId, request.Quantity);
        if (request.Quantity <= 0)
        {
            var remove = await cartRepository.RemoveShoppingCartItemAsync(request.CartItemId, cancellationToken);
            return remove.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(remove.ErrorMessage ?? "Failed to remove item");
        }

        var result = await cartRepository.UpdateShoppingCartItemQuantityAsync(request.CartItemId, request.Quantity, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to update quantity");
    }
}


