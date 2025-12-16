using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class GetProductsByShopPagingHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsByShopPagingHandler> logger
) : IRequestHandler<GetProductsByShopPagingQuery, Result<PagedResult<ProductBaseResponse>>>
{
    public async Task<Result<PagedResult<ProductBaseResponse>>> Handle(GetProductsByShopPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsByShopPagingQuery - ShopId: {ShopId}, Page: {Page}, PageSize: {PageSize}", request.ShopId, request.Request.Page, request.Request.PageSize);

        var result = await repository.GetByShopIdAsync(request.Request, request.ShopId, cancellationToken);
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


