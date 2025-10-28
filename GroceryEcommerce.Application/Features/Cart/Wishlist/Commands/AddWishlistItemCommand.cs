using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.Wishlist.Commands;

public record AddWishlistItemCommand(
    Guid UserId,
    Guid ProductId,
    Guid? ProductVariantId
) : IRequest<Result<bool>>;

