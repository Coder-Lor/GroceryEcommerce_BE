// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class GetProductAttributeByIdQueryHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<GetProductAttributeByIdQueryHandler> logger
) : IRequestHandler<GetProductAttributeByIdQuery, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(GetProductAttributeByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product attribute by id {AttributeId}", request.AttributeId);

        var result = await repository.GetByIdAsync(request.AttributeId, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<ProductAttributeDto>.Failure(result.ErrorMessage ?? "Failed to get attribute");
        }

        if (result.Data == null)
        {
            return Result<ProductAttributeDto>.Failure($"Attribute with id '{request.AttributeId}' not found.");
        }

        var mapped = mapper.Map<ProductAttributeDto>(result.Data);
        return Result<ProductAttributeDto>.Success(mapped);
    }
}
// ...existing code...
