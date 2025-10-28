using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Queries;

public record GetUnansweredQuestionsQuery(
    int Page,
    int PageSize,
    string? SortBy,
    string? SortDirection
) : IRequest<Result<PagedResult<ProductQuestionDto>>>;
