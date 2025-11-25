using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class GetGiftCardByIdQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardByIdQueryHandler> logger
) : IRequestHandler<GetGiftCardByIdQuery, Result<GiftCardDto?>>
{
    public async Task<Result<GiftCardDto?>> Handle(GetGiftCardByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCard by Id: {GiftCardId}", request.GiftCardId);

        var result = await repository.GetByIdAsync(request.GiftCardId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCard: {GiftCardId}", request.GiftCardId);
            return Result<GiftCardDto?>.Failure(result.ErrorMessage ?? "Failed to get GiftCard.");
        }

        if (result.Data == null)
        {
            return Result<GiftCardDto?>.Success(null);
        }

        var dto = mapper.Map<GiftCardDto>(result.Data);
        return Result<GiftCardDto?>.Success(dto);
    }
}

