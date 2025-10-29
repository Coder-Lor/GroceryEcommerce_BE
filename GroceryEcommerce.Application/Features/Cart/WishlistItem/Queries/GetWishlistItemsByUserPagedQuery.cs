using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.WishlistItem.Queries;

public record GetWishlistItemsByUserPagedQuery(
    PagedRequest Request,
    Guid UserId
) : IRequest<Result<PagedResult<WishlistItemDto>>>;

