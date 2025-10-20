using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductQuestion.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Handlers;

public class UpdateProductQuestionCommandHandler(
    IProductQuestionRepository repository,
    IMapper mapper,
    ILogger<UpdateProductQuestionCommandHandler> logger
) : IRequestHandler<UpdateProductQuestionCommand, Result<UpdateProductQuestionResponse>>
{
    public async Task<Result<UpdateProductQuestionResponse>> Handle(UpdateProductQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating product question {QuestionId}", request.QuestionId);

        var existing = await repository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (!existing.IsSuccess || existing.Data is null)
        {
            return Result<UpdateProductQuestionResponse>.Failure("Product question not found");
        }

        existing.Data.Question = request.Question;
        existing.Data.Status = request.Status;

        var updateResult = await repository.UpdateAsync(existing.Data, cancellationToken);
        if (!updateResult.IsSuccess)
        {
            return Result<UpdateProductQuestionResponse>.Failure(updateResult.ErrorMessage ?? "Failed to update product question");
        }

        var response = mapper.Map<UpdateProductQuestionResponse>(existing.Data);
        return Result<UpdateProductQuestionResponse>.Success(response);
    }
}
