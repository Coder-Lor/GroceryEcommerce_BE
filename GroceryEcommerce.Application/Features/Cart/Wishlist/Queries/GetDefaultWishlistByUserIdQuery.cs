using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Queries;

public record GetDefaultWishlistByUserIdQuery(Guid UserId) : IRequest<Result<WishlistDto>>;

