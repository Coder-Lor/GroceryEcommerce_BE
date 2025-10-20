using AutoMapper;
using GroceryEcommerce.Application.Common;
using GroceryEcommerce.Application.Features.ProductImage.Commands;
using GroceryEcommerce.Application.Interfaces.Repositories.Catalog;
using GroceryEcommerce.Application.Models.Catalog;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GroceryEcommerce.Application.Features.ProductImage.Handlers;

public class CreateProductImageCommandHandler(
    IProductImageRepository repository,
    IMapper mapper,
    ILogger<CreateProductImageCommandHandler> logger
) : IRequestHandler<CreateProductImageCommand, Result<CreateProductImageResponse>>
{
    public async Task<Result<CreateProductImageResponse>> Handle(CreateProductImageCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating product image for product {ProductId}", request.ProductId);

        var createReq = new CreateProductImageRequest
        {
            ProductId = request.ProductId,
            ImageUrl = request.ImageUrl,
            AltText = request.AltText,
            DisplayOrder = request.DisplayOrder,
            IsPrimary = request.IsPrimary
        };

        var entity = mapper.Map<Domain.Entities.Catalog.ProductImage>(createReq);

        var result = await repository.CreateAsync(entity, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to create product image for product {ProductId}: {Error}", request.ProductId, result.ErrorMessage);
            return Result<CreateProductImageResponse>.Failure(result.ErrorMessage ?? "Failed to create product image");
        }

        var response = mapper.Map<CreateProductImageResponse>(result.Data);
        return Result<CreateProductImageResponse>.Success(response);
    }
}


