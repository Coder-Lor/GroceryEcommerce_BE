using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Handlers;

public class GetWishlistItemsPagedHandler(
    ICartRepository cartRepository,
    IMapper mapper,
    ILogger<GetWishlistItemsPagedHandler> logger
) : IRequestHandler<GetWishlistItemsPagedQuery, Result<PagedResult<WishlistItemDto>>>
{
    public async Task<Result<PagedResult<WishlistItemDto>>> Handle(GetWishlistItemsPagedQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting wishlist items paged for wishlist {WishlistId}", request.WishlistId);

        var result = await cartRepository.GetWishlistItemsAsync(request.Request, request.WishlistId, cancellationToken);
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

