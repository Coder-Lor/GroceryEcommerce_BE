using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.Catalog.ProductQuestion.Handlers;

public class CreateProductQuestionCommandHandler(
    IProductQuestionRepository repository,
    IMapper mapper,
    ILogger<CreateProductQuestionCommandHandler> logger
) : IRequestHandler<CreateProductQuestionCommand, Result<CreateProductQuestionResponse>>
{
    public async Task<Result<CreateProductQuestionResponse>> Handle(CreateProductQuestionCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product question for product {ProductId}", request.ProductId);

        var createReq = new CreateProductQuestionRequest
        {
            ProductId = request.ProductId,
            UserId = request.UserId,
            Question = request.Question,
            Status = request.Status
        };

        var entity = mapper.Map<Domain.Entities.Catalog.ProductQuestion>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<CreateProductQuestionResponse>.Failure(result.ErrorMessage ?? "Failed to create product question");
        }

        var response = mapper.Map<CreateProductQuestionResponse>(result.Data);
        return Result<CreateProductQuestionResponse>.Success(response);
    }
}
