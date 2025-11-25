using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class GetGiftCardByCodeQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardByCodeQueryHandler> logger
) : IRequestHandler<GetGiftCardByCodeQuery, Result<GiftCardDto?>>
{
    public async Task<Result<GiftCardDto?>> Handle(GetGiftCardByCodeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCard by Code: {Code}", request.Code);

        var result = await repository.GetByCodeAsync(request.Code, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCard by Code: {Code}", request.Code);
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

