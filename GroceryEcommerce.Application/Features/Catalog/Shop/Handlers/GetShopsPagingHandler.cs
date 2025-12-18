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
        
        // Query ProductCount và OrderCount từ database cho mỗi shop
        foreach (var shopDto in response.Items)
        {
            if (shopDto.ShopId != Guid.Empty)
            {
                try
                {
                    // Query ProductCount
                    var productCountResult = await repository.GetProductCountByShopAsync(shopDto.ShopId, cancellationToken);
                    if (productCountResult.IsSuccess)
                    {
                        shopDto.ProductCount = productCountResult.Data;
                    }
                    
                    // Query OrderCount
                    var orderCountResult = await repository.GetOrderCountByShopAsync(shopDto.ShopId, cancellationToken);
                    if (orderCountResult.IsSuccess)
                    {
                        shopDto.OrderCount = orderCountResult.Data;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error querying counts for shop: {ShopId}", shopDto.ShopId);
                    // Giữ giá trị mặc định từ mapping (0)
                }
            }
        }
        
        return Result<PagedResult<ShopDto>>.Success(response);
    }
}


