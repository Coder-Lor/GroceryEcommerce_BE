using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Commands;

public record MarkAbandonedCartsNotifiedCommand(List<Guid> AbandonedCartIds) : IRequest<Result<bool>>;

