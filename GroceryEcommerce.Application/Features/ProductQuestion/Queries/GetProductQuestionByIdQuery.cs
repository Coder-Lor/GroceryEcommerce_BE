using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Queries;

public record GetProductQuestionByIdQuery(
    Guid QuestionId
) : IRequest<Result<ProductQuestionDto>>;
