using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductAttributeValue.Handlers;

public class GetProductAttributeValuesByProductHandler(
    IProductAttributeValueRepository repository,
    IMapper mapper,
    ILogger<GetProductAttributeValuesByProductHandler> logger
) : IRequestHandler<GetProductAttributeValuesByProductQuery, Result<PagedResult<ProductAttributeValueDto>>>
{
    public async Task<Result<PagedResult<ProductAttributeValueDto>>> Handle(GetProductAttributeValuesByProductQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting attribute values for product {ProductId}", request.ProductId);
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
            return Result<PagedResult<ProductAttributeValueDto>>.Failure(result.ErrorMessage ?? "Failed to get attribute values");
        }

        var mapped = mapper.Map<PagedResult<ProductAttributeValueDto>>(result.Data);
        return Result<PagedResult<ProductAttributeValueDto>>.Success(mapped);
    }
}
