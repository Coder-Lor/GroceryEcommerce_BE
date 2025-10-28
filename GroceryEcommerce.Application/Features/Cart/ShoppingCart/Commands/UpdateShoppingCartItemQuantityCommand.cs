using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;

public record UpdateShoppingCartItemQuantityCommand(
    Guid CartItemId,
    int Quantity
) : IRequest<Result<bool>>;


