using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Queries;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductImage.Handlers;

public class GetProductImageByIdHandler(
    IProductImageRepository repository,
    IMapper mapper,
    ILogger<GetProductImageByIdHandler> logger
) : IRequestHandler<GetProductImageByIdQuery, Result<ProductImageDto>>
{
    public async Task<Result<ProductImageDto>> Handle(GetProductImageByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting product image by id {ImageId}", request.ImageId);
        var result = await repository.GetByIdAsync(request.ImageId, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            logger.LogWarning("Product image not found: {ImageId}", request.ImageId);
            return Result<ProductImageDto>.Failure("Product image not found");
        }

        var dto = mapper.Map<ProductImageDto>(result.Data);
        return Result<ProductImageDto>.Success(dto);
    }
}


