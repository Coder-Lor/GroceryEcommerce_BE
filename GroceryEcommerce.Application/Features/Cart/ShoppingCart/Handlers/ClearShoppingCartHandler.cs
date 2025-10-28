using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Handlers;

public class ClearShoppingCartHandler(
    ICartRepository cartRepository,
    ILogger<ClearShoppingCartHandler> logger
) : IRequestHandler<ClearShoppingCartCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ClearShoppingCartCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Clearing shopping cart for user {UserId}", request.UserId);

        var result = await cartRepository.ClearShoppingCartAsync(request.UserId, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to clear cart");
    }
}

