using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Commands;

public record MarkAbandonedCartNotifiedCommand(Guid AbandonedCartId) : IRequest<Result<bool>>;

