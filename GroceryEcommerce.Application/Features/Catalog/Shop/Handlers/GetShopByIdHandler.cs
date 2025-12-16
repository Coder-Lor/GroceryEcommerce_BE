using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Shop.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Shop.Handlers;

public class GetShopByIdHandler(
    IMapper mapper,
    IShopRepository repository,
    ILogger<GetShopByIdHandler> logger
) : IRequestHandler<GetShopByIdQuery, Result<GetShopByIdResponse>>
{
    public async Task<Result<GetShopByIdResponse>> Handle(GetShopByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetShopByIdQuery for shop: {ShopId}", request.ShopId);

        var shopResult = await repository.GetByIdAsync(request.ShopId, cancellationToken);
        if (!shopResult.IsSuccess || shopResult.Data is null)
        {
            logger.LogWarning("Shop not found: {ShopId}", request.ShopId);
            return Result<GetShopByIdResponse>.Failure("Shop not found");
        }

        var response = mapper.Map<GetShopByIdResponse>(shopResult.Data);
        return Result<GetShopByIdResponse>.Success(response);
    }
}


