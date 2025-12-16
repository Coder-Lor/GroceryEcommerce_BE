using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class GetShopsPagingHandler(
    IMapper mapper,
    IShopRepository repository,
    ILogger<GetShopsPagingHandler> logger
) : IRequestHandler<GetShopsPagingQuery, Result<PagedResult<ShopDto>>>
{
    public async Task<Result<PagedResult<ShopDto>>> Handle(GetShopsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetShopsPagingQuery with Page: {Page}, PageSize: {PageSize}", request.Request.Page, request.Request.PageSize);

        var shopResult = await repository.GetPagedAsync(request.Request, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found for the given criteria.");
            return Result<PagedResult<ShopDto>>.Failure("Shop not found");
        }

        var response = mapper.Map<PagedResult<ShopDto>>(shopResult.Data);
        return Result<PagedResult<ShopDto>>.Success(response);
    }
}


