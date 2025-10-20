using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductQuestion.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Handlers;

public class GetProductQuestionsByProductHandler(
    IProductQuestionRepository repository,
    IMapper mapper,
    ILogger<GetProductQuestionsByProductHandler> logger
) : IRequestHandler<GetProductQuestionsByProductQuery, Result<PagedResult<ProductQuestionDto>>>
{
    public async Task<Result<PagedResult<ProductQuestionDto>>> Handle(GetProductQuestionsByProductQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product questions for product {ProductId}", request.ProductId);
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
            return Result<PagedResult<ProductQuestionDto>>.Failure(result.ErrorMessage ?? "Failed to get product questions");
        }

        var mapped = mapper.Map<PagedResult<ProductQuestionDto>>(result.Data);
        return Result<PagedResult<ProductQuestionDto>>.Success(mapped);
    }
}
