using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductImage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductImage.Handlers;

public class GetProductImagesPagingHandler(
    IProductImageRepository repository,
    IMapper mapper,
    ILogger<GetProductImagesPagingHandler> logger
) : IRequestHandler<GetProductImagesPagingQuery, Result<PagedResult<ProductImageDto>>>
{
    public async Task<Result<PagedResult<ProductImageDto>>> Handle(GetProductImagesPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product images for product {ProductId} page {Page}", request.ProductId, request.Page);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetByProductIdAsync(pagedRequest, request.ProductId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductImageDto>>.Failure(result.ErrorMessage ?? "Failed to get product images");
        }

        var mapped = mapper.Map<PagedResult<ProductImageDto>>(result.Data);
        return Result<PagedResult<ProductImageDto>>.Success(mapped);
    }
}
