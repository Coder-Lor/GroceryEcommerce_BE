// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class GetProductAttributeByNameQueryHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<GetProductAttributeByNameQueryHandler> logger
) : IRequestHandler<GetProductAttributeByNameQuery, Result<ProductAttributeDto>>
{
    public async Task<Result<ProductAttributeDto>> Handle(GetProductAttributeByNameQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product attribute by name {Name}", request.Name);

        var result = await repository.GetByNameAsync(request.Name, cancellationToken);
        if (!result.IsSuccess)
        {
            return Result<ProductAttributeDto>.Failure(result.ErrorMessage ?? "Failed to get attribute");
        }

        if (result.Data == null)
        {
            return Result<ProductAttributeDto>.Failure($"Attribute with name '{request.Name}' not found.");
        }

        var mapped = mapper.Map<ProductAttributeDto>(result.Data);
        return Result<ProductAttributeDto>.Success(mapped);
    }
}
// ...existing code...
