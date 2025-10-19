using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Product.Handlers;

public class GetProductsPagingHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsPagingHandler> logger
) : IRequestHandler<GetProductsPagingQuery, Result<GetProductsPagingResponse>>
{
    public async Task<Result<GetProductsPagingResponse>> Handle(GetProductsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsPagingQuery - Page: {Page}, PageSize: {PageSize}", request.Page, request.PageSize);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filters
        if (request.Status.HasValue)
        {
            pagedRequest.WithFilter("Status", request.Status.Value);
        }

        if (request.CategoryId.HasValue)
        {
            pagedRequest.WithFilter("CategoryId", request.CategoryId.Value);
        }

        if (request.BrandId.HasValue)
        {
            pagedRequest.WithFilter("BrandId", request.BrandId.Value);
        }

        if (request.IsFeatured.HasValue)
        {
            pagedRequest.WithFilter("IsFeatured", request.IsFeatured.Value);
        }

        var result = await repository.SearchProductsAsync(pagedRequest, request.SearchTerm ?? string.Empty, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get paged products");
            return Result<GetProductsPagingResponse>.Failure(result.ErrorMessage ?? "Failed to get paged products.");
        }

        var response = mapper.Map<GetProductsPagingResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} products for page {Page}", result.Data?.TotalCount ?? 0, request.Page);
        return Result<GetProductsPagingResponse>.Success(response);
    }
}
