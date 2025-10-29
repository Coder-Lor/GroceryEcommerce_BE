using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Handlers;

public class GetUnansweredQuestionsHandler(
    IProductQuestionRepository repository,
    IMapper mapper,
    ILogger<GetUnansweredQuestionsHandler> logger
) : IRequestHandler<GetUnansweredQuestionsQuery, Result<PagedResult<ProductQuestionDto>>>
{
    public async Task<Result<PagedResult<ProductQuestionDto>>> Handle(GetUnansweredQuestionsQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting unanswered questions");
        var pagedRequest = new PagedRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection == "Desc" ? SortDirection.Descending : SortDirection.Ascending
        };

        var result = await repository.GetUnansweredQuestionsAsync(pagedRequest, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductQuestionDto>>.Failure(result.ErrorMessage ?? "Failed to get unanswered questions");
        }

        var mapped = mapper.Map<PagedResult<ProductQuestionDto>>(result.Data);
        return Result<PagedResult<ProductQuestionDto>>.Success(mapped);
    }
}
