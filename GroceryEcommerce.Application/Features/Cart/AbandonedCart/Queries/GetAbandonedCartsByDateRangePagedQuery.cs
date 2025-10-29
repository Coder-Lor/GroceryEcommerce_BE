using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Cart;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Queries;

public record GetAbandonedCartsByDateRangePagedQuery(
    PagedRequest Request,
    DateTime FromDate,
    DateTime ToDate
) : IRequest<Result<PagedResult<AbandonedCartDto>>>;

