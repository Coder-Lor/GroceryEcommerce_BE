using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;

public record GetWishlistItemsPagedQuery(
    PagedRequest Request,
    Guid WishlistId
) : IRequest<Result<PagedResult<WishlistItemDto>>>;

