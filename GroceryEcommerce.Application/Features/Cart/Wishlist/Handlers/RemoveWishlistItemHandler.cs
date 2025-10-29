using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Handlers;

public class RemoveWishlistItemHandler(
    ICartRepository cartRepository,
    ILogger<RemoveWishlistItemHandler> logger
) : IRequestHandler<RemoveWishlistItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RemoveWishlistItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing wishlist item {WishlistItemId}", request.WishlistItemId);

        var result = await cartRepository.RemoveWishlistItemAsync(request.WishlistItemId, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to remove item");
    }
}

