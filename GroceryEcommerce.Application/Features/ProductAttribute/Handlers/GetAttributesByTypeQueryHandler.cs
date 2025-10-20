// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class GetAttributesByTypeQueryHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<GetAttributesByTypeQueryHandler> logger
) : IRequestHandler<GetAttributesByTypeQuery, Result<PagedResult<ProductAttributeDto>>>
{
    public async Task<Result<PagedResult<ProductAttributeDto>>> Handle(GetAttributesByTypeQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product attributes by type {AttributeType}", request.AttributeType);

        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetByTypeAsync(pagedRequest, request.AttributeType, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductAttributeDto>>.Failure(result.ErrorMessage ?? "Failed to get attributes by type");
        }

        var mapped = mapper.Map<PagedResult<ProductAttributeDto>>(result.Data);
        return Result<PagedResult<ProductAttributeDto>>.Success(mapped);
    }
}
// ...existing code...
