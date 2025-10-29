using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.AbandonedCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Handlers;

public class MarkAbandonedCartsNotifiedHandler(
    IAbandonedCartRepository abandonedCartRepository,
    ILogger<MarkAbandonedCartsNotifiedHandler> logger
) : IRequestHandler<MarkAbandonedCartsNotifiedCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkAbandonedCartsNotifiedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking {Count} abandoned carts as notified", request.AbandonedCartIds.Count);

        var result = await abandonedCartRepository.MarkAsNotifiedAsync(request.AbandonedCartIds, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to mark as notified");
    }
}

