using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;

public record AddShoppingCartItemCommand(
    Guid ProductId,
    Guid? ProductVariantId,
    int Quantity
) : IRequest<Result<bool>>;


