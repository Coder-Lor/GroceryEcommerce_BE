using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetLowStockProductsHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetLowStockProductsHandler> logger
) : IRequestHandler<GetLowStockProductsQuery, Result<GetLowStockProductsResponse>>
{
    public async Task<Result<GetLowStockProductsResponse>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetLowStockProductsQuery - Threshold: {Threshold}, Page: {Page}", request.Threshold, request.Page);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filter for low stock (StockQuantity <= Threshold)
        pagedRequest.WithFilter("StockQuantity", request.Threshold);

        var result = await repository.GetLowStockProductsAsync(pagedRequest, request.Threshold, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get low stock products with threshold: {Threshold}", request.Threshold);
            return Result<GetLowStockProductsResponse>.Failure(result.ErrorMessage ?? "Failed to get low stock products.");
        }

        var response = mapper.Map<GetLowStockProductsResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} low stock products (threshold: {Threshold}), page {Page}", 
            result.Data?.TotalCount ?? 0, request.Threshold, request.Page);
        return Result<GetLowStockProductsResponse>.Success(response);
    }
}
