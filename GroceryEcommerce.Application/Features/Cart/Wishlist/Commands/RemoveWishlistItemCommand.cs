using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;

public record RemoveWishlistItemCommand(Guid WishlistItemId) : IRequest<Result<bool>>;

