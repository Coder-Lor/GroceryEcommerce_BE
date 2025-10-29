using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;

public record GetShoppingCartByUserIdQuery(Guid UserId) : IRequest<Result<ShoppingCartDto>>;


