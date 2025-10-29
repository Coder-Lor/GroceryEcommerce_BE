using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;

public record RemoveShoppingCartItemCommand(Guid CartItemId) : IRequest<Result<bool>>;


