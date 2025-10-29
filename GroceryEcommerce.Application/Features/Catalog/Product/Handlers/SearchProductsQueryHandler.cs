using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class SearchProductsQueryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<SearchProductsQueryHandler> logger
) : IRequestHandler<SearchProductsQuery, Result<SearchProductsResponse>>
{
    public async Task<Result<SearchProductsResponse>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Searching products with term: {SearchTerm}", request.SearchTerm);

            // Create paged request
            var pagedRequest = new PagedRequest
            {
                Page = request.Page,
                PageSize = request.PageSize,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
            };

            // Add filters based on request parameters
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

            if (request.IsActive.HasValue)
            {
                pagedRequest.WithFilter("Status", request.IsActive.Value ? 1 : 0);
            }

            // Execute search
            var result = await repository.SearchProductsAsync(pagedRequest, request.SearchTerm ?? string.Empty, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to search products");
                return Result<SearchProductsResponse>.Failure(result.ErrorMessage ?? "Failed to search products.");
            }

            // Map to response
            var response = mapper.Map<SearchProductsResponse>(result.Data);

            logger.LogInformation("Product search completed: {Count} results", result.Data?.TotalCount ?? 0);
            return Result<SearchProductsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching products: {SearchTerm}", request.SearchTerm);
            return Result<SearchProductsResponse>.Failure("An error occurred while searching products.");
        }
    }
}
