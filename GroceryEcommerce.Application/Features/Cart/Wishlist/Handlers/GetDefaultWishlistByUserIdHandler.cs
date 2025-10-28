using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Handlers;

public class GetDefaultWishlistByUserIdHandler(
    IWishlistRepository wishlistRepository,
    IMapper mapper,
    ILogger<GetDefaultWishlistByUserIdHandler> logger
) : IRequestHandler<GetDefaultWishlistByUserIdQuery, Result<WishlistDto>>
{
    public async Task<Result<WishlistDto>> Handle(GetDefaultWishlistByUserIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting default wishlist for user {UserId}", request.UserId);

        var result = await wishlistRepository.GetDefaultWishlistByUserIdAsync(request.UserId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<WishlistDto>.Failure(result.ErrorMessage ?? "Wishlist not found");
        }

        var dto = mapper.Map<WishlistDto>(result.Data);
        return Result<WishlistDto>.Success(dto);
    }
}

