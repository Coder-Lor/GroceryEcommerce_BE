using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class UpdateGiftCardCommandHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<UpdateGiftCardCommandHandler> logger
) : IRequestHandler<UpdateGiftCardCommand, Result<GiftCardDto>>
{
    public async Task<Result<GiftCardDto>> Handle(UpdateGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating GiftCard: {GiftCardId}", request.GiftCardId);

        var existingResult = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!existingResult.IsSuccess || existingResult.Data == null)
        {
            return Result<GiftCardDto>.Failure("GiftCard not found.");
        }

        var giftCard = existingResult.Data;
        giftCard.Balance = request.InitialAmount;
        giftCard.ExpiresAt = request.ValidTo;
        giftCard.IsActive = request.Status == 1;

        var updateResult = await repository.UpdateAsync(giftCard, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            logger.LogError("Failed to update GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<GiftCardDto>.Failure(updateResult.ErrorMessage ?? "Failed to update GiftCard.");
        }

        var updatedResult = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!updatedResult.IsSuccess || updatedResult.Data == null)
        {
            return Result<GiftCardDto>.Failure("Failed to retrieve updated GiftCard.");
        }

        var dto = mapper.Map<GiftCardDto>(updatedResult.Data);
        logger.LogInformation("GiftCard updated: {GiftCardId}", request.GiftCardId);
        return Result<GiftCardDto>.Success(dto);
    }
}

