using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class GetProductsByPriceRangeHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsByPriceRangeHandler> logger
) : IRequestHandler<GetProductsByPriceRangeQuery, Result<GetProductsByPriceRangeResponse>>
{
    public async Task<Result<GetProductsByPriceRangeResponse>> Handle(GetProductsByPriceRangeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsByPriceRangeQuery - MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Page: {Page}", 
            request.MinPrice, request.MaxPrice, request.Page);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filters for price range
        pagedRequest.WithFilter("MinPrice", request.MinPrice);
        pagedRequest.WithFilter("MaxPrice", request.MaxPrice);

        var result = await repository.GetProductsByPriceRangeAsync(pagedRequest, request.MinPrice, request.MaxPrice, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get products by price range: {MinPrice}-{MaxPrice}", request.MinPrice, request.MaxPrice);
            return Result<GetProductsByPriceRangeResponse>.Failure(result.ErrorMessage ?? "Failed to get products by price range.");
        }

        var response = mapper.Map<GetProductsByPriceRangeResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} products in price range {MinPrice}-{MaxPrice}, page {Page}", 
            result.Data?.TotalCount ?? 0, request.MinPrice, request.MaxPrice, request.Page);
        return Result<GetProductsByPriceRangeResponse>.Success(response);
    }
}
