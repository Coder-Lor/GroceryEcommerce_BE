using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.Product.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.Product.Handlers;

public class GetProductsByCategoryHandler(
    IMapper mapper,
    IProductRepository repository,
    ILogger<GetProductsByCategoryHandler> logger
) : IRequestHandler<GetProductsByCategoryQuery, Result<GetProductsByCategoryResponse>>
{
    public async Task<Result<GetProductsByCategoryResponse>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling GetProductsByCategoryQuery for category: {CategoryId}, Page: {Page}", request.CategoryId, request.Page);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        // Add filter for category
        pagedRequest.WithFilter("CategoryId", request.CategoryId);

        var result = await repository.GetByCategoryIdAsync(pagedRequest, request.CategoryId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Failed to get products by category: {CategoryId}", request.CategoryId);
            return Result<GetProductsByCategoryResponse>.Failure(result.ErrorMessage ?? "Failed to get products by category.");
        }

        var response = mapper.Map<GetProductsByCategoryResponse>(result.Data);
        logger.LogInformation("Retrieved {Count} products for category {CategoryId}, page {Page}", result.Data?.TotalCount ?? 0, request.CategoryId, request.Page);
        return Result<GetProductsByCategoryResponse>.Success(response);
    }
}
