using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductVariant.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductVariant.Handlers;

public class GetProductVariantByIdHandler(
    IProductVariantRepository repository,
    IMapper mapper,
    ILogger<GetProductVariantByIdHandler> logger
) : IRequestHandler<GetProductVariantByIdQuery, Result<ProductVariantDto>>
{
    public async Task<Result<ProductVariantDto>> Handle(GetProductVariantByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product variant by id {VariantId}", request.VariantId);
        var result = await repository.GetByIdAsync(request.VariantId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            return Result<ProductVariantDto>.Failure("Product variant not found");
        }
        var dto = mapper.Map<ProductVariantDto>(result.Data);
        return Result<ProductVariantDto>.Success(dto);
    }
}
