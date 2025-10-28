using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Commands;

public record ClearShoppingCartCommand(Guid UserId) : IRequest<Result<bool>>;

