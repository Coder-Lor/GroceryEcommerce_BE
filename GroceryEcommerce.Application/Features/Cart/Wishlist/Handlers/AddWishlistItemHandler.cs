using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Handlers;

public class AddWishlistItemHandler(
    ICartRepository cartRepository,
    ILogger<AddWishlistItemHandler> logger
) : IRequestHandler<AddWishlistItemCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AddWishlistItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding item to wishlist for user {UserId}, product {ProductId}", request.UserId, request.ProductId);

        var wishlistResult = await cartRepository.GetWishlistByUserIdAsync(request.UserId, cancellationToken);
        Domain.Entities.Cart.Wishlist wishlist;
        if (!wishlistResult.IsSuccess || wishlistResult.Data is null)
        {
            wishlist = new Domain.Entities.Cart.Wishlist
            {
                WishlistId = Guid.NewGuid(),
                UserId = request.UserId,
                Name = "Default Wishlist",
                IsDefault = true,
                CreatedAt = DateTime.UtcNow
            };
            var createWishlist = await cartRepository.CreateWishlistAsync(wishlist, cancellationToken);
            if (!createWishlist.IsSuccess)
            {
                return Result<bool>.Failure(createWishlist.ErrorMessage ?? "Failed to create wishlist");
            }
            wishlist = createWishlist.Data!;
        }
        else
        {
            wishlist = wishlistResult.Data;
        }

        var existingItem = await cartRepository.GetWishlistItemByProductAsync(wishlist.WishlistId, request.ProductId, request.ProductVariantId, cancellationToken);
        if (existingItem.IsSuccess && existingItem.Data is not null)
        {
            return Result<bool>.Failure("Product already in wishlist");
        }

        var item = new Domain.Entities.Cart.WishlistItem
        {
            WishlistItemId = Guid.NewGuid(),
            WishlistId = wishlist.WishlistId,
            ProductId = request.ProductId,
            ProductVariantId = request.ProductVariantId,
            CreatedAt = DateTime.UtcNow
        };

        var addRes = await cartRepository.AddWishlistItemAsync(item, cancellationToken);
        return addRes.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(addRes.ErrorMessage ?? "Failed to add item");
    }
}

