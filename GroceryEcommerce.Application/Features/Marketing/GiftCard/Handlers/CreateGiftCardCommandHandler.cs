using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class CreateGiftCardCommandHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<CreateGiftCardCommandHandler> logger
) : IRequestHandler<CreateGiftCardCommand, Result<GiftCardDto>>
{
    public async Task<Result<GiftCardDto>> Handle(CreateGiftCardCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating GiftCard with InitialAmount: {InitialAmount}", request.InitialAmount);

        var code = GenerateGiftCardCode();

        var existsResult = await repository.ExistsAsync(code, cancellationToken);
        if (existsResult.IsSuccess && existsResult.Data)
        {
            code = GenerateGiftCardCode();
        }

        var giftCard = new Domain.Entities.Marketing.GiftCard
        {
            GiftCardId = Guid.NewGuid(),
            Code = code,
            InitialAmount = request.InitialAmount,
            Balance = request.InitialAmount,
            ExpiresAt = request.ValidTo,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.PurchasedBy
        };

        var result = await repository.CreateAsync(giftCard, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to create GiftCard");
            return Result<GiftCardDto>.Failure(result.ErrorMessage ?? "Failed to create GiftCard.");
        }

        if (result.Data is null)
        {
            logger.LogError("GiftCard creation returned null entity");
            return Result<GiftCardDto>.Failure("Failed to create GiftCard.");
        }

        var dto = mapper.Map<GiftCardDto>(result.Data);
        logger.LogInformation("GiftCard created: {GiftCardId}, Code: {Code}", result.Data.GiftCardId, code);
        return Result<GiftCardDto>.Success(dto);
    }

    private static string GenerateGiftCardCode()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

