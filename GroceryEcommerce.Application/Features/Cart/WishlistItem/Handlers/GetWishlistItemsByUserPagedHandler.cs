using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.WishlistItem.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.WishlistItem.Handlers;

public class GetWishlistItemsByUserPagedHandler(
    IWishlistItemRepository wishlistItemRepository,
    IMapper mapper,
    ILogger<GetWishlistItemsByUserPagedHandler> logger
) : IRequestHandler<GetWishlistItemsByUserPagedQuery, Result<PagedResult<WishlistItemDto>>>
{
    public async Task<Result<PagedResult<WishlistItemDto>>> Handle(GetWishlistItemsByUserPagedQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wishlist items paged for user {UserId}", request.UserId);

        var result = await wishlistItemRepository.GetByUserIdAsync(request.Request, request.UserId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<WishlistItemDto>>.Failure(result.ErrorMessage ?? "Failed to get wishlist items");
        }

        var mappedItems = mapper.Map<List<WishlistItemDto>>(result.Data.Items);
        var response = new PagedResult<WishlistItemDto>(
            mappedItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );

        return Result<PagedResult<WishlistItemDto>>.Success(response);
    }
}

