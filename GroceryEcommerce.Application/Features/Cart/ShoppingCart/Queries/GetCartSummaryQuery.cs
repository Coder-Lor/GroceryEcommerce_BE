using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.ShoppingCart.Queries;

public record GetCartSummaryQuery(Guid UserId) : IRequest<Result<CartSummaryDto>>;

