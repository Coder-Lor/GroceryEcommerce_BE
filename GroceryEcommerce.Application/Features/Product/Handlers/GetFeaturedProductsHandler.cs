using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetFeaturedProductsHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetFeaturedProductsHandler> logger
) : IRequestHandler<GetFeaturedProductsQuery, Result<GetFeaturedProductsResponse>>
{
    public async Task<Result<GetFeaturedProductsResponse>> Handle(GetFeaturedProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetFeaturedProductsQuery - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filter for featured products
        pagedRequest.WithFilter("IsFeatured", true);

        var result = await repository.GetFeaturedProductsAsync(pagedRequest, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get featured products");
            return Result<GetFeaturedProductsResponse>.Failure(result.ErrorMessage ?? "Failed to get featured products.");
        }

        var response = mapper.Map<GetFeaturedProductsResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} featured products for page {Page}", result.Data?.TotalCount ?? 0, request.Page);
        return Result<GetFeaturedProductsResponse>.Success(response);
    }
}
