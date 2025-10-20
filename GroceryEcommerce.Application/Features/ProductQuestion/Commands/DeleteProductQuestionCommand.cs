using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Commands;

public record DeleteProductQuestionCommand(
    Guid QuestionId
) : IRequest<Result<bool>>;
