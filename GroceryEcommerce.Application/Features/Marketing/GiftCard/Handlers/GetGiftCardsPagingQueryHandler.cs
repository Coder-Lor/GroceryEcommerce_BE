using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Marketing.GiftCard.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Marketing;
using GroceryEcommerce.Application.Models.Marketing;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Marketing.GiftCard.Handlers;

public class GetGiftCardsPagingQueryHandler(
    IGiftCardRepository repository,
    IMapper mapper,
    ILogger<GetGiftCardsPagingQueryHandler> logger
) : IRequestHandler<GetGiftCardsPagingQuery, Result<PagedResult<GiftCardDto>>>
{
    public async Task<Result<PagedResult<GiftCardDto>>> Handle(GetGiftCardsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting GiftCards paging - Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var result = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get GiftCards paging");
            return Result<PagedResult<GiftCardDto>>.Failure(result.ErrorMessage ?? "Failed to get GiftCards.");
        }

        var dtos = mapper.Map<List<GiftCardDto>>(result.Data?.Items ?? new List<Domain.Entities.Marketing.GiftCard>());
        var pagedResult = new PagedResult<GiftCardDto>(
            dtos,
            result.Data?.TotalCount ?? 0,
            result.Data?.Page ?? request.Request.Page,
            result.Data?.PageSize ?? request.Request.PageSize
        );

        return Result<PagedResult<GiftCardDto>>.Success(pagedResult);
    }
}

