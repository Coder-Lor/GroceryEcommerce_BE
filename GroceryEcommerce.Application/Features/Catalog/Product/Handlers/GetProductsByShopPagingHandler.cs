using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Interfaces.Services;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class GetProductsByShopPagingHandler(
    IMapper mapper,
    IProductRepository repository,
    ICurrentUserService currentUserService,
    ILogger<GetProductsByShopPagingHandler> logger
) : IRequestHandler<GetProductsByShopPagingQuery, Result<PagedResult<ProductBaseResponse>>>
{
    public async Task<Result<PagedResult<ProductBaseResponse>>> Handle(GetProductsByShopPagingQuery request, CancellationToken cancellationToken)
    {
        // Nếu không truyền ShopId (Guid.Empty), tự lấy theo user hiện tại
        var shopId = request.ShopId;
        if (shopId == Guid.Empty)
        {
            var currentShopId = currentUserService.GetCurrentUserShopId();
            if (currentShopId is null || currentShopId == Guid.Empty)
            {
                logger.LogWarning("GetProductsByShopPagingQuery - No ShopId provided and current user has no shop. UserId: {UserId}", currentUserService.GetCurrentUserId());
                return Result<PagedResult<ProductBaseResponse>>.Failure("Shop not found for current user.");
            }

            shopId = currentShopId.Value;
        }

        logger.LogInformation("Handling GetProductsByShopPagingQuery - ShopId: {ShopId}, Page: {Page}, PageSize: {PageSize}", shopId, request.Request.Page, request.Request.PageSize);

        var result = await repository.GetByShopIdAsync(request.Request, shopId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductBaseResponse>>.Failure(result.ErrorMessage ?? "Failed to get products by shop.");
        }

        var mappedItems = mapper.Map<List<ProductBaseResponse>>(result.Data.Items);
        var response = new PagedResult<ProductBaseResponse>(
            mappedItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );

        return Result<PagedResult<ProductBaseResponse>>.Success(response);
    }
}


