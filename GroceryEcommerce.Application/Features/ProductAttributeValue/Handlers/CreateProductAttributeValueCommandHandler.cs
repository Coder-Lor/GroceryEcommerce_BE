using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttributeValue.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Handlers;

public class CreateProductAttributeValueCommandHandler(
    IProductAttributeValueRepository repository,
    IMapper mapper,
    ILogger<CreateProductAttributeValueCommandHandler> logger
) : IRequestHandler<CreateProductAttributeValueCommand, Result<CreateProductAttributeValueResponse>>
{
    public async Task<Result<CreateProductAttributeValueResponse>> Handle(CreateProductAttributeValueCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product attribute value for product {ProductId}, attribute {AttributeId}", request.ProductId, request.AttributeId);

        var createReq = new CreateProductAttributeValueRequest
        {
            ProductId = request.ProductId,
            ProductAttributeId = request.AttributeId,
            Value = request.Value
        };

        var entity = mapper.Map<Domain.Entities.Catalog.ProductAttributeValue>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<CreateProductAttributeValueResponse>.Failure(result.ErrorMessage ?? "Failed to create attribute value");
        }

        var response = mapper.Map<CreateProductAttributeValueResponse>(result.Data);
        return Result<CreateProductAttributeValueResponse>.Success(response);
    }
}


