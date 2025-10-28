using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductVariant.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductVariant.Handlers;

public class GetLowStockVariantsHandler(
    IProductVariantRepository repository,
    IMapper mapper,
    ILogger<GetLowStockVariantsHandler> logger
) : IRequestHandler<GetLowStockVariantsQuery, Result<PagedResult<ProductVariantDto>>>
{
    public async Task<Result<PagedResult<ProductVariantDto>>> Handle(GetLowStockVariantsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting low stock variants with threshold {Threshold}", request.Threshold);
        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetLowStockVariantsAsync(pagedRequest, request.Threshold, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductVariantDto>>.Failure(result.ErrorMessage ?? "Failed to get low stock variants");
        }

        var mapped = mapper.Map<PagedResult<ProductVariantDto>>(result.Data);
        return Result<PagedResult<ProductVariantDto>>.Success(mapped);
    }
}
