using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class DeleteGiftCardCommandHandler(
    IGiftCardRepository repository,
    ILogger<DeleteGiftCardCommandHandler> logger
) : IRequestHandler<DeleteGiftCardCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting GiftCard: {GiftCardId}", request.GiftCardId);

        var result = await repository.DeleteAsync(request.GiftCardId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to delete GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to delete GiftCard.");
        }

        logger.LogInformation("GiftCard deleted: {GiftCardId}", request.GiftCardId);
        return Result<bool>.Success(true);
    }
}

