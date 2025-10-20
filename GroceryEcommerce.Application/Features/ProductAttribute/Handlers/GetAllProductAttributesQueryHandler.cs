// ...existing code...
using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductAttribute.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductAttribute.Handlers;

public class GetAllProductAttributesQueryHandler(
    IProductAttributeRepository repository,
    IMapper mapper,
    ILogger<GetAllProductAttributesQueryHandler> logger
) : IRequestHandler<GetAllProductAttributesQuery, Result<PagedResult<ProductAttributeDto>>>
{
    public async Task<Result<PagedResult<ProductAttributeDto>>> Handle(GetAllProductAttributesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all product attributes");

        var result = await repository.GetAllAsync(cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<PagedResult<ProductAttributeDto>>.Failure(result.ErrorMessage ?? "Failed to get attributes");
        }

        var mapped = mapper.Map<IReadOnlyList<ProductAttributeDto>>(result.Data);
        var pageSize = mapped.Count == 0 ? 1 : mapped.Count;
        var paged = new PagedResult<ProductAttributeDto>(mapped, mapped.Count, 1, pageSize);

        return Result<PagedResult<ProductAttributeDto>>.Success(paged);
    }
}
// ...existing code...
