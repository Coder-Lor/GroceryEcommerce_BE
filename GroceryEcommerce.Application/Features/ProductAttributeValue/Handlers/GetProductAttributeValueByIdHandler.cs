using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttributeValue.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttributeValue.Handlers;

public class GetProductAttributeValueByIdHandler(
    IProductAttributeValueRepository repository,
    IMapper mapper,
    ILogger<GetProductAttributeValueByIdHandler> logger
) : IRequestHandler<GetProductAttributeValueByIdQuery, Result<ProductAttributeValueDto>>
{
    public async Task<Result<ProductAttributeValueDto>> Handle(GetProductAttributeValueByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product attribute value by id {ValueId}", request.ValueId);
        var result = await repository.GetByIdAsync(request.ValueId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ProductAttributeValueDto>.Failure("Attribute value not found");
        }
        var dto = mapper.Map<ProductAttributeValueDto>(result.Data);
        return Result<ProductAttributeValueDto>.Success(dto);
    }
}


