using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetActiveProductsHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetActiveProductsHandler> logger
) : IRequestHandler<GetActiveProductsQuery, Result<GetActiveProductsResponse>>
{
    public async Task<Result<GetActiveProductsResponse>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetActiveProductsQuery - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filter for active products (Status = 1)
        pagedRequest.WithFilter("Status", 1);

        var result = await repository.GetActiveProductsAsync(pagedRequest, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get active products");
            return Result<GetActiveProductsResponse>.Failure(result.ErrorMessage ?? "Failed to get active products.");
        }

        var response = mapper.Map<GetActiveProductsResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} active products for page {Page}", result.Data?.TotalCount ?? 0, request.Page);
        return Result<GetActiveProductsResponse>.Success(response);
    }
}
