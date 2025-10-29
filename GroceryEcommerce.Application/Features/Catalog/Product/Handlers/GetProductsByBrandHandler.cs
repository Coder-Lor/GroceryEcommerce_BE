using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class GetProductsByBrandHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsByBrandHandler> logger
) : IRequestHandler<GetProductsByBrandQuery, Result<GetProductsByBrandResponse>>
{
    public async Task<Result<GetProductsByBrandResponse>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsByBrandQuery for brand: {BrandId}, Page: {Page}", request.BrandId, request.Page);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filter for brand
        pagedRequest.WithFilter("BrandId", request.BrandId);

        var result = await repository.GetByBrandIdAsync(pagedRequest, request.BrandId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get products by brand: {BrandId}", request.BrandId);
            return Result<GetProductsByBrandResponse>.Failure(result.ErrorMessage ?? "Failed to get products by brand.");
        }

        var response = mapper.Map<GetProductsByBrandResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} products for brand {BrandId}, page {Page}", result.Data?.TotalCount ?? 0, request.BrandId, request.Page);
        return Result<GetProductsByBrandResponse>.Success(response);
    }
}
