using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductQuestion.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductQuestion.Handlers;

public class GetProductQuestionByIdHandler(
    IProductQuestionRepository repository,
    IMapper mapper,
    ILogger<GetProductQuestionByIdHandler> logger
) : IRequestHandler<GetProductQuestionByIdQuery, Result<ProductQuestionDto>>
{
    public async Task<Result<ProductQuestionDto>> Handle(GetProductQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product question by id {QuestionId}", request.QuestionId);
        var result = await repository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ProductQuestionDto>.Failure("Product question not found");
        }
        var dto = mapper.Map<ProductQuestionDto>(result.Data);
        return Result<ProductQuestionDto>.Success(dto);
    }
}
