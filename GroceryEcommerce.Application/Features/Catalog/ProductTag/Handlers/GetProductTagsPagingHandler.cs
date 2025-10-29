using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductTag.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductTag.Handlers;

public class GetProductTagsPagingHandler(
    IProductTagRepository repository,
    IMapper mapper,
    ILogger<GetProductTagsPagingHandler> logger
) : IRequestHandler<GetProductTagsPagingQuery, Result<PagedResult<ProductTagDto>>>
{
    public async Task<Result<PagedResult<ProductTagDto>>> Handle(GetProductTagsPagingQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product tags paging");
        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending,
            Search = request.SearchTerm
        };

        var result = await repository.SearchByNameAsync(pagedRequest, request.SearchTerm ?? string.Empty, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductTagDto>>.Failure(result.ErrorMessage ?? "Failed to get product tags");
        }

        var mapped = mapper.Map<PagedResult<ProductTagDto>>(result.Data);
        return Result<PagedResult<ProductTagDto>>.Success(mapped);
    }
}
