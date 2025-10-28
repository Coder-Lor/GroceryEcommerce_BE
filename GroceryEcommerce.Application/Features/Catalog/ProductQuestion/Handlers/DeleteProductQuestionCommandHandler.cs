using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Handlers;

public class DeleteProductQuestionCommandHandler(
    IProductQuestionRepository repository,
    ILogger<DeleteProductQuestionCommandHandler> logger
) : IRequestHandler<DeleteProductQuestionCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteProductQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting product question {QuestionId}", request.QuestionId);

        var exists = await repository.ExistsAsync(request.QuestionId, cancellationToken);
        if (!exists.IsSuccess || !exists.Data)
        {
            return Result<bool>.Failure("Product question not found");
        }

        var del = await repository.DeleteAsync(request.QuestionId, cancellationToken);
        if (!del.IsSuccess || !del.Data)
        {
            return Result<bool>.Failure(del.ErrorMessage ?? "Failed to delete product question");
        }

        return Result<bool>.Success(true);
    }
}
