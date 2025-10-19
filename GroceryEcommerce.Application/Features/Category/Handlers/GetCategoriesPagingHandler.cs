using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Category.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Category.Handlers;

public class GetCategoriesPagingHandler(
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<GetCategoriesPagingHandler> logger
) : IRequestHandler<GetCategoriesPagingQuery, Result<PagedResult<CategoryBaseResponse>>>
{
    public async Task<Result<PagedResult<CategoryBaseResponse>>> Handle(GetCategoriesPagingQuery request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Getting categories paged: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

            var pagedRequest = new PagedRequest
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.SearchTerm,
                SortBy = request.SortBy,
                SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
            };

            if (request.Status.HasValue)
                pagedRequest.WithFilter("Status", request.Status.Value);

            if (request.ParentCategoryId.HasValue)
                pagedRequest.WithFilter("ParentCategoryId", request.ParentCategoryId.Value);

            var result = await categoryRepository.GetPagedAsync(pagedRequest, cancellationToken);
            if (!result.IsSuccess)
            {
                logger.LogError("Failed to get paged categories");
                return Result<PagedResult<CategoryBaseResponse>>.Failure(result.ErrorMessage ?? "Failed to get categories.");
            }

            var mapped = mapper.Map<PagedResult<CategoryBaseResponse>>(result.Data);
            return Result<PagedResult<CategoryBaseResponse>>.Success(mapped);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting paged categories");
            return Result<PagedResult<CategoryBaseResponse>>.Failure("An error occurred while retrieving categories.");
        }
    }
}
