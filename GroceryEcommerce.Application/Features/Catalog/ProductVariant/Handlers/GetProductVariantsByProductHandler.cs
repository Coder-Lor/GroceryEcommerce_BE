using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class GetProductVariantsByProductHandler(
    IProductVariantRepository repository,
    IMapper mapper,
    ILogger<GetProductVariantsByProductHandler> logger
) : IRequestHandler<GetProductVariantsByProductQuery, Result<PagedResult<ProductVariantDto>>>
{
    public async Task<Result<PagedResult<ProductVariantDto>>> Handle(GetProductVariantsByProductQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product variants for product {ProductId}", request.ProductId);
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
            return Result<PagedResult<ProductVariantDto>>.Failure(result.ErrorMessage ?? "Failed to get product variants");
        }

        var mappedItems = mapper.Map<List<ProductVariantDto>>(result.Data.Items);
        var response = new PagedResult<ProductVariantDto>(
            mappedItems,
            result.Data.TotalCount,
            result.Data.Page,
            result.Data.PageSize
        );
        return Result<PagedResult<ProductVariantDto>>.Success(response);
    }
}
