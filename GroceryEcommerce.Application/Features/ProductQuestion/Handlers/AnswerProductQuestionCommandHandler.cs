using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductQuestion.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Handlers;

public class AnswerProductQuestionCommandHandler(
    IProductQuestionRepository repository,
    ILogger<AnswerProductQuestionCommandHandler> logger
) : IRequestHandler<AnswerProductQuestionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(AnswerProductQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Answering product question {QuestionId}", request.QuestionId);

        var result = await repository.AnswerQuestionAsync(request.QuestionId, request.Answer, request.AnsweredBy, cancellationToken);
        if (!result.IsSuccess || !result.Data)
        {
            return Result<bool>.Failure(result.ErrorMessage ?? "Failed to answer product question");
        }

        return Result<bool>.Success(true);
    }
}
