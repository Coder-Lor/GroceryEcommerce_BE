using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Commands;

public record AnswerProductQuestionCommand(
    Guid QuestionId,
    string Answer,
    Guid AnsweredBy
) : IRequest<Result<bool>>;
