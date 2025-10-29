using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class RemoveShoppingCartItemHandler(
    ICartRepository cartRepository,
    ILogger<RemoveShoppingCartItemHandler> logger
) : IRequestHandler<RemoveShoppingCartItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveShoppingCartItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing cart item {CartItemId}", request.CartItemId);

        var result = await cartRepository.RemoveShoppingCartItemAsync(request.CartItemId, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to remove item");
    }
}

