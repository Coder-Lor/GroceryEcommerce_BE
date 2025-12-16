using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class GetShopsByOwnerHandler(
    IMapper mapper,
    IShopRepository repository,
    ILogger<GetShopsByOwnerHandler> logger
) : IRequestHandler<GetShopsByOwnerQuery, Result<PagedResult<ShopDto>>>
{
    public async Task<Result<PagedResult<ShopDto>>> Handle(GetShopsByOwnerQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetShopsByOwnerQuery for owner: {OwnerUserId}", request.OwnerUserId);

        var shopResult = await repository.GetByOwnerAsync(request.OwnerUserId, request.Request, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shops not found for owner: {OwnerUserId}", request.OwnerUserId);
            return Result<PagedResult<ShopDto>>.Failure("Shops not found");
        }

        var response = mapper.Map<PagedResult<ShopDto>>(shopResult.Data);
        return Result<PagedResult<ShopDto>>.Success(response);
    }
}


