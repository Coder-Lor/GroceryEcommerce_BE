using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Queries;

public record GetUnnotifiedAbandonedCartsPagedQuery(PagedRequest Request) : IRequest<Result<PagedResult<AbandonedCartDto>>>;

