// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class GetRequiredAttributesQueryHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<GetRequiredAttributesQueryHandler> logger
) : IRequestHandler<GetRequiredAttributesQuery, Result<PagedResult<ProductAttributeDto>>>
{
    public async Task<Result<PagedResult<ProductAttributeDto>>> Handle(GetRequiredAttributesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting required product attributes");

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetRequiredAttributesAsync(pagedRequest, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductAttributeDto>>.Failure(result.ErrorMessage ?? "Failed to get required attributes");
        }

        var mapped = mapper.Map<PagedResult<ProductAttributeDto>>(result.Data);
        return Result<PagedResult<ProductAttributeDto>>.Success(mapped);
    }
}
// ...existing code...
