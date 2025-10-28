using GroceryEcommerce.Application.Common;
using MediatR;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Commands;

public record DeleteProductQuestionCommand(
    Guid QuestionId
) : IRequest<Result<bool>>;
