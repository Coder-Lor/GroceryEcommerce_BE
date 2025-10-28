using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Cart.AbandonedCart.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Cart;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Cart.AbandonedCart.Handlers;

public class MarkAbandonedCartNotifiedHandler(
    IAbandonedCartRepository abandonedCartRepository,
    ILogger<MarkAbandonedCartNotifiedHandler> logger
) : IRequestHandler<MarkAbandonedCartNotifiedCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkAbandonedCartNotifiedCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Marking abandoned cart {AbandonedCartId} as notified", request.AbandonedCartId);

        var result = await abandonedCartRepository.MarkAsNotifiedAsync(request.AbandonedCartId, cancellationToken);
        return result.IsSuccess ? Result<bool>.Success(true) : Result<bool>.Failure(result.ErrorMessage ?? "Failed to mark as notified");
    }
}

